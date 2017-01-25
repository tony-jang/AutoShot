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
using System.Threading;

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

        public Vector RealSize;
        Thickness RealThickness;



        double _Ratio = 100.0;
        public double Ratio
        {
            get { return _Ratio; }
            set
            {
                _Ratio = value;

                Vector AfterSize = new Vector(RealSize.X * (_Ratio / 100), RealSize.Y * (_Ratio / 100));

                MainGrid.Width = AfterSize.X + 16;
                MainGrid.Height = AfterSize.Y + 16;

                PreviewRect.Width = AfterSize.X + 16;
                PreviewRect.Height = AfterSize.Y + 16;

                CropGrid.Width = AfterSize.X + 16;
                CropGrid.Height = AfterSize.Y + 16;



                DragGrid.Margin = new Thickness(RealThickness.Left * (Ratio / 100),
                                                RealThickness.Top * (Ratio / 100),
                                                RealThickness.Right * (Ratio / 100),
                                                RealThickness.Bottom * (Ratio / 100));

                RatioChanged();
            }

        }


        public delegate void SizeChangedEventHandler();
        public event SizeChangedEventHandler ImageSizeChanged;

        public delegate void RatioChangedEventHandler();
        public event RatioChangedEventHandler RatioChanged;


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
        f.Timer Movetmr = new f.Timer();


        Rectangle[] CropRects;
        Rectangle[] DragRects;

        #region 외부 노출 속성


        private BitmapSource _Originalimage;
        /// <summary>
        /// 최초의 이미지입니다. 잘린게 아닌 Image 자체가 변경될때만 해당 필드를 변경해야 합니다.
        /// </summary>
        public BitmapSource Originalimage
        {
            get { return _Originalimage; }
            set
            {
                _Originalimage = value;
                RealImage = value;
            }
        }

        /// <summary>
        /// 실제로 보여지는 이미지 입니다.
        /// </summary>
        private BitmapSource RealImage
        {
            get { return (BitmapSource)InnerImg.Source; }
            set
            {
                InnerImg.Source = value;

                SourceSize = new Size(value.Width, value.Height);
                RealSize = (Vector)SourceSize;
                if (SourceSize.Width < this.Width)
                {
                    Ratio = 50;
                }
                MainGrid.Width = SourceSize.Width + 16;
                MainGrid.Height = SourceSize.Height + 16;

                CropGrid.Width = SourceSize.Width + 16;
                CropGrid.Height = SourceSize.Height + 16;

                ImageSizeChanged();
                PreviewRect.Width = value.Width;
                PreviewRect.Height = value.Height;
                DragGrid.Margin = new Thickness(0, 0, 0, 0);
            }
        }

        public void ResetToOringinal()
        {
            Ratio = 100;
            RealImage = Originalimage;
        }
        public Size VisibleSize
        {
            get { return new Size(MainGrid.Width - 16, MainGrid.Height - 16); }
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
                    case EditMode.PenEdit:
                        foreach (Rectangle rect in DragRects) rect.Visibility = Visibility.Hidden;
                        CropGrid.Visibility = Visibility.Hidden;
                        break;
                }
            }
        }
        #endregion



        public ImageEdit()
        {
            InitializeComponent();


            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.HighQuality);

            CropRects = new Rectangle[]{UL_Cropper, UC_Cropper, UR_Cropper,
                                     CL_Cropper, CR_Cropper,
                                     DL_Cropper, DC_Cropper, DR_Cropper};

            DragRects = new Rectangle[] { RC_Dragger, RUC_Dragger, RD_Dragger };


            foreach (Rectangle CropRect in CropRects)
                CropRect.MouseDown += Cropper_Process;

            foreach (Rectangle DragRect in DragRects)
                DragRect.MouseDown += Dragger_Process;


            CropRect.MouseDown += CropRect_MouseDown;

            Dragtmr.Interval = 10; Croptmr.Interval = 10; Movetmr.Interval = 1;

            Dragtmr.Tick += Tick_SizeChange;
            Croptmr.Tick += Tick_CropChange;
            Movetmr.Tick += Tick_MoveChange;

        }

        private void Tick_MoveChange(object sender, EventArgs e)
        {
            if (f.Control.MouseButtons == f.MouseButtons.Left)
            {
                WIN32POINT NowPosition;
                GetCursorPos(out NowPosition);

                int MovedX, MovedY;
                MovedX = NowPosition.X - StartPosition.X;
                MovedY = NowPosition.Y - StartPosition.Y;

                Thickness ChangeThickness = new Thickness(0);

                ChangeThickness.Left = GetMargin(DragGrid.Margin, ThickPos.Left, StartMargin.Left + MovedX, CropGrid.Width - DragGrid.Width, false).Left;
                ChangeThickness.Right = GetMargin(DragGrid.Margin, ThickPos.Right, StartMargin.Right - MovedX, CropGrid.Width - DragGrid.Width, false).Right;
                ChangeThickness.Top = GetMargin(DragGrid.Margin, ThickPos.Top, StartMargin.Top + MovedY, CropGrid.Height - DragGrid.Height, false).Top;
                ChangeThickness.Bottom = GetMargin(DragGrid.Margin, ThickPos.Bottom, StartMargin.Bottom - MovedY, CropGrid.Height - DragGrid.Height, false).Bottom;

                if ((ChangeThickness.Left < 0)) { ChangeThickness.Right = ChangeThickness.Right - Math.Abs(ChangeThickness.Left); ChangeThickness.Left = 0; }
                if ((ChangeThickness.Right < 0)) { ChangeThickness.Left = ChangeThickness.Left - Math.Abs(ChangeThickness.Right); ChangeThickness.Right = 0; }
                if ((ChangeThickness.Top < 0)) { ChangeThickness.Bottom = ChangeThickness.Bottom - Math.Abs(ChangeThickness.Top); ChangeThickness.Top = 0; }
                if ((ChangeThickness.Bottom < 0)) { ChangeThickness.Top = ChangeThickness.Top - Math.Abs(ChangeThickness.Bottom); ChangeThickness.Bottom = 0; }

                DragGrid.Margin = ChangeThickness;

                RealThickness = new Thickness(DragGrid.Margin.Left * (100 / Ratio),
                                              DragGrid.Margin.Top * (100 / Ratio),
                                              DragGrid.Margin.Right * (100 / Ratio),
                                              DragGrid.Margin.Bottom * (100 / Ratio));

                //DragGrid

            }
            else
            {
                MoveUpProcess();
            }
        }

        private void MoveUpProcess()
        {

            Movetmr.Stop();
        }

        private void CropRect_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetCursorPos(out StartPosition);

            StartMargin = DragGrid.Margin;

            Movetmr.Start();
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

                if (IncMode.HasFlag(IncreaseMode.DownIncrease))
                {
                    DragGrid.Margin = GetMargin(DragGrid.Margin, ThickPos.Bottom,
                        AddedHeight + StartMargin.Bottom, CropGrid.ActualHeight - DragGrid.Margin.Top - 11);

                }
                else if (IncMode.HasFlag(IncreaseMode.UpIncrease))
                {
                    DragGrid.Margin = GetMargin(DragGrid.Margin, ThickPos.Top,
                        -AddedHeight + StartMargin.Top, CropGrid.ActualHeight - DragGrid.Margin.Bottom - 11);
                }

                if (IncMode.HasFlag(IncreaseMode.LeftIncrease))
                {
                    DragGrid.Margin = GetMargin(DragGrid.Margin, ThickPos.Left,
                        -AddedWidth + StartMargin.Left, CropGrid.ActualWidth - DragGrid.Margin.Right - 11);
                }
                else if (IncMode.HasFlag(IncreaseMode.RightIncrease))
                {
                    DragGrid.Margin = GetMargin(DragGrid.Margin, ThickPos.Right,
                        AddedWidth + StartMargin.Right, CropGrid.ActualWidth - DragGrid.Margin.Left - 11);
                }

                RealThickness = new Thickness(DragGrid.Margin.Left * (100 / Ratio),
                                              DragGrid.Margin.Top * (100 / Ratio),
                                              DragGrid.Margin.Right * (100 / Ratio),
                                              DragGrid.Margin.Bottom * (100 / Ratio));
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

                if (Mode == DragMode.Both || Mode == DragMode.OnlyWidth)
                {
                    if (Width < 16) PreviewRect.Width = 0;
                    else PreviewRect.Width = Width - 16;
                    ReservePoint.Width = Width;
                }
                if (Mode == DragMode.Both || Mode == DragMode.OnlyHeight)
                {
                    if (Height < 16) PreviewRect.Height = 0;
                    else PreviewRect.Height = Height - 16;
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
            if (Mode == DragMode.Both || Mode == DragMode.OnlyWidth)
            {
                MainGrid.Width = ReservePoint.Width;
                CropGrid.Width = ReservePoint.Width;
                RealSize.X = (ReservePoint.Width - 16) * (100 / Ratio);
            }
            if (Mode == DragMode.Both || Mode == DragMode.OnlyHeight)
            {
                MainGrid.Height = ReservePoint.Height;
                CropGrid.Height = ReservePoint.Height;
                RealSize.Y = (ReservePoint.Height - 16) * (100 / Ratio);
            }
            Mode = DragMode.Wait;

            //MessageBox.Show(DragGrid.Margin.ToString());
            StartPosition.X = 0; StartPosition.Y = 0;
            this.Cursor = null;
            Dragtmr.Stop();
            if (ImageSizeChanged != null) ImageSizeChanged();
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

                BitmapImage img = new BitmapImage();

                img.BeginInit();
                PngBitmapEncoder enc = new PngBitmapEncoder();
                img.StreamSource = new MemoryStream(ImageSourceToBytes(enc, RealImage));

                img.CacheOption = BitmapCacheOption.OnLoad;
                img.DecodePixelWidth = (int)RealSize.X;
                img.DecodePixelHeight = (int)RealSize.Y;
                img.EndInit();

                CropImg = new CroppedBitmap(img, new Int32Rect((int)RealThickness.Left, (int)RealThickness.Top,
                                                          (int)(CropRect.ActualWidth * (100 / Ratio)), (int)(CropRect.ActualHeight * (100 / Ratio))));

                RealThickness = new Thickness(0);
                Ratio = 100;
                RealImage = CropImg;


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
        SizeChange, PenEdit, CropImage
    }
}
