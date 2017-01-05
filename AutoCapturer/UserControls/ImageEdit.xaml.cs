using System;
using System.Runtime.InteropServices;
using System.Windows;
using f = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AutoCapturer.Globals.Globals;
using System.IO;

namespace AutoCapturer.UserControls
{
    /// <summary>
    /// ImageEdit.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageEdit : System.Windows.Controls.UserControl
    {

        public struct WIN32POINT
        {
            public int X;
            public int Y;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out WIN32POINT lpPoint);

        // StretchMode를 None으로 설정했을때 이미지 최대 크기
        Size MaximumSize;
        // 이미지 크기
        Size SourceSize;

        Size BoardSize;
        Size ReservePoint;

        Thickness StartMargin;

        WIN32POINT StartPosition;
        DragMode Mode = DragMode.Wait;
        IncreaseMode IncMode = IncreaseMode.None;

        f.Timer Dragtmr = new f.Timer();
        f.Timer Croptmr = new f.Timer();


        Rectangle[] CropRects;
        Rectangle[] DragRects;
        
        #region 외부 노출 속성

        public BitmapSource image
        {
            get { return (BitmapSource)InnerImg.Source; }
            set
            {
                InnerImg.Source = value;
                SourceSize = new Size(value.Width, value.Height);
                ImageStretchMode = Stretch.None;
                DragGrid.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// 이미지를 늘리는 모드를 의미합니다.
        /// </summary>
        public Stretch ImageStretchMode
        {
            get { return InnerImg.Stretch; }
            set
            {
                InnerImg.Stretch = value;
                switch (value)
                {
                    case Stretch.None:
                        MaximumSize = new Size(SourceSize.Width + 11, SourceSize.Height + 11);
                        MainGrid.Width = MaximumSize.Width;
                        MainGrid.Height = MaximumSize.Height;
                        CropGrid.Width = MaximumSize.Width;
                        CropGrid.Height = MaximumSize.Height;
                        break;
                    case Stretch.Fill:
                        MaximumSize = new Size(0, 0);
                        break;
                    case Stretch.Uniform:
                        MaximumSize = new Size(0, 0);
                        break;
                }
            }
        }

        private EditMode _ImageEditMode;

        /// <summary>
        /// 이미지 변경하는 모드를 의미합니다.
        /// </summary>
        public EditMode ImageEditMode
        {
            get { return _ImageEditMode; }
            set
            {
                _ImageEditMode = value;
                switch (value)
                {
                    case EditMode.SizeChange:
                        foreach (Rectangle rect in DragRects) rect.Visibility = Visibility.Visible;
                        CropGrid.Visibility = Visibility.Hidden;
                        break;
                    case EditMode.CropImage:
                        foreach (Rectangle rect in DragRects) rect.Visibility = Visibility.Hidden;
                        CropGrid.Visibility = Visibility.Visible;
                        break;
                }
            }
        }
        #endregion



        public ImageEdit()
        {
            InitializeComponent();

            CropRects = new Rectangle[]{UL_Cropper, UC_Cropper, UR_Cropper,
                                     CL_Cropper, CR_Cropper,
                                     DL_Cropper, DC_Cropper, DR_Cropper};

            DragRects = new Rectangle[]{ RC_Dragger, RUC_Dragger, RD_Dragger };


            foreach (Rectangle CropRect in CropRects)
                CropRect.MouseDown += Cropper_Process;
            
            foreach (Rectangle DragRect in DragRects)
                DragRect.MouseDown += Dragger_Process;
            
            

            Dragtmr.Interval = 10; Croptmr.Interval = 10;

            Dragtmr.Tick += Tick_SizeChange;
            Croptmr.Tick += Tick_CropChange;

        }

        public enum ThickPos
        {
            Bottom,Top,Left,Right
        }

        public Thickness GetMargin(Thickness margin, ThickPos thickpos,double value, double Maximum = 1000,bool DisAllowMinus = true, bool AllowOverMaximum = false)
        {
            double l = margin.Left, t = margin.Top, r = margin.Right, b = margin.Bottom;
            bool lb = false, tb = false, rb = false, bb = false;
            Console.WriteLine($"{thickpos.ToString()} :: {value}");
            switch (thickpos)
            {
                case ThickPos.Left:   l = value; lb = true; break;
                case ThickPos.Top:    t = value; tb = true; break;
                case ThickPos.Right:  r = value; rb = true; break;
                case ThickPos.Bottom: b = value; bb = true; break;
            }
            if (DisAllowMinus)
            {
                if (l < 0 && lb) l = 0;
                if (t < 0 && tb) t = 0;
                if (r < 0 && rb) r = 0;
                if (b < 0 && bb) b = 0;
            }
            if (!AllowOverMaximum)
            {
                if (l > Maximum && lb) l = Maximum;
                if (t > Maximum && tb) t = Maximum;
                if (r > Maximum && rb) r = Maximum;
                if (b > Maximum && bb) b = Maximum;
            }
            return new Thickness(l, t, r, b);
        }

        private void Tick_CropChange(object sender, EventArgs e)
        {
            if (f.Control.MouseButtons == f.MouseButtons.Left)
            {
                WIN32POINT NowPosition;
                GetCursorPos(out NowPosition);

                
                double AddedWidth, AddedHeight;

                AddedWidth = StartPosition.X - NowPosition.X;
                AddedHeight = StartPosition.Y - NowPosition.Y;


                //Console.WriteLine($"{AddedWidth} :: {AddedHeight}");
    
                if (IncMode.HasFlag(IncreaseMode.DownIncrease))
                    DragGrid.Margin = GetMargin(DragGrid.Margin, ThickPos.Bottom,
                        AddedHeight + StartMargin.Bottom, CropGrid.ActualHeight - DragGrid.Margin.Top - 11);
                
                else if (IncMode.HasFlag(IncreaseMode.UpIncrease))
                    DragGrid.Margin = GetMargin(DragGrid.Margin, ThickPos.Top,
                        -AddedHeight + StartMargin.Top, CropGrid.ActualHeight - DragGrid.Margin.Bottom - 11); 
                if (IncMode.HasFlag(IncreaseMode.LeftIncrease))
                    DragGrid.Margin = GetMargin(DragGrid.Margin, ThickPos.Left,
                        -AddedWidth + StartMargin.Left, CropGrid.ActualWidth - DragGrid.Margin.Right - 11);
                else if (IncMode.HasFlag(IncreaseMode.RightIncrease))
                    DragGrid.Margin = GetMargin(DragGrid.Margin, ThickPos.Right,
                        AddedWidth + StartMargin.Right, CropGrid.ActualWidth - DragGrid.Margin.Left - 11);
                
            }
            else
            {
                CropUpProcess();
            }
        }
        public void CropUpProcess()
        {
            PreviewRect.Opacity = 0.0;
            Mode = DragMode.Wait;
            this.Cursor = null;
            
            Croptmr.Stop();
        }

        private void Tick_SizeChange(object sender, EventArgs e)
        {
            if (f.Control.MouseButtons == f.MouseButtons.Left)
            {
                WIN32POINT NowPosition;
                GetCursorPos(out NowPosition);
                double Width, Height;
                Width = BoardSize.Width - (StartPosition.X - NowPosition.X);
                Height = BoardSize.Height - (StartPosition.Y - NowPosition.Y);
                // 최소 사이즈 (Margin 5(10) + 1)
                if (Width < 11) Width = 11; if (Height < 11) Height = 11;
                // 최대 사이즈
                if (MaximumSize.Width != 0 && MaximumSize.Height != 0) { 
                    if (Width >= MaximumSize.Width) Width = MaximumSize.Width;
                    if (Height >= MaximumSize.Height) Height = MaximumSize.Height;
                }
                if (Mode == DragMode.Both || Mode == DragMode.OnlyWidth)
                {
                    PreviewRect.Width = Width - 10;
                    ReservePoint.Width = Width;
                }
                if (Mode == DragMode.Both || Mode == DragMode.OnlyHeight)
                {
                    PreviewRect.Height = Height - 10;
                    ReservePoint.Height = Height;
                }
            }
            else
            {
                DragUpProcess();
            }
        }

        public void DragUpProcess()
        {
            PreviewRect.Opacity = 0.0;
            if (Mode == DragMode.Both || Mode == DragMode.OnlyWidth) { MainGrid.Width = ReservePoint.Width; CropGrid.Width = ReservePoint.Width; }
            if (Mode == DragMode.Both || Mode == DragMode.OnlyHeight) { MainGrid.Height = ReservePoint.Height; CropGrid.Height = ReservePoint.Height; }
            Mode = DragMode.Wait;
            StartPosition.X = 0; StartPosition.Y = 0;
            this.Cursor = null;
            Dragtmr.Stop();
        }

        private void Dragger_Process(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = ((Rectangle)sender).Cursor;
            GetCursorPos(out StartPosition);
            BoardSize = new Size(MainGrid.Width, MainGrid.Height);
            PreviewRect.Opacity = 1.0;
            switch ((string)((Rectangle)sender).Tag)
            {
                case "Width":
                    Mode = DragMode.OnlyWidth;
                    break;
                case "Height":
                    Mode = DragMode.OnlyHeight;
                    break;
                case "Both":
                    Mode = DragMode.Both;
                    break;
            }
            Dragtmr.Start();
        }

        private void Cropper_Process(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = ((Rectangle)sender).Cursor;
            GetCursorPos(out StartPosition);

            BoardSize = new Size(MainGrid.Width, MainGrid.Height);

            switch (((Rectangle)sender).Name.Substring(0, 2))
            {
                case "UL":
                    IncMode = IncreaseMode.LeftUp; break;
                case "UC":
                    IncMode = IncreaseMode.UpIncrease; break;
                case "UR":
                    IncMode = IncreaseMode.RightUp; break;
                case "CL":
                    IncMode = IncreaseMode.LeftIncrease; break;
                case "CR":
                    IncMode = IncreaseMode.RightIncrease; break;
                case "DL":
                    IncMode = IncreaseMode.LeftDown; break;
                case "DC":
                    IncMode = IncreaseMode.DownIncrease; break;
                case "DR":
                    IncMode = IncreaseMode.RightDown; break;
            }
            StartMargin = DragGrid.Margin;

            Croptmr.Start();
        }
        private void BgGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (f.Control.MouseButtons == f.MouseButtons.Right)
            {
                if (ImageEditMode != EditMode.CropImage) return;


                CroppedBitmap CropImg = null;

                if (ImageStretchMode == Stretch.Fill)
                {
                    BitmapImage img = new BitmapImage();

                    img.BeginInit();
                    PngBitmapEncoder enc = new PngBitmapEncoder();
                    img.StreamSource = new MemoryStream(ImageSourceToBytes(enc, image));

                    img.CacheOption = BitmapCacheOption.OnLoad;
                    img.DecodePixelWidth = (int)MainGrid.Width - 10;
                    img.DecodePixelHeight = (int)MainGrid.Height - 10;
                    img.EndInit();

                    CropImg = new CroppedBitmap(img, new Int32Rect((int)DragGrid.Margin.Left, (int)DragGrid.Margin.Top,
                                                              (int)(DragGrid.ActualWidth - 11), (int)(DragGrid.ActualHeight - 11)));

                    image = CropImg;
                }

                else if (ImageStretchMode == Stretch.None)
                {
                    

                    if (DragGrid.Margin != new Thickness(0, 0, 0, 0))
                    {
                        CropImg = new CroppedBitmap(image, new Int32Rect((int)DragGrid.Margin.Left, (int)DragGrid.Margin.Top,
                                                              (int)(DragGrid.ActualWidth - 11), (int)(DragGrid.ActualHeight - 11)));
                    }

                    else return;

                    image = CropImg;
                }
                
            }
        }
    }


    public enum DragMode
    {
        OnlyWidth, OnlyHeight, Both, Wait
    }
    
    [Flags]
    public enum IncreaseMode
    {
        None = 0,
        LeftIncrease = 1 << 1,
        RightIncrease = 1 << 2,
        DownIncrease = 1 << 3,
        UpIncrease = 1 << 4,

        LeftDown = LeftIncrease | DownIncrease,
        LeftUp = LeftIncrease | UpIncrease,

        RightDown = RightIncrease | DownIncrease,
        RightUp = RightIncrease | UpIncrease
        
          
    }

    public enum EditMode
    {
        SizeChange, Select, CropImage
    }
}
