using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using f=System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace AutoCapturer.UserControls
{
    /// <summary>
    /// ImageEdit.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageEdit : System.Windows.Controls.UserControl
    {
        // 외부 공개용 속성

        public Stretch ImageStretchMode
        {
            get { return InnerImg.Stretch; }
            set { InnerImg.Stretch = value; }
        }

        private EditMode _ImageEditMode;
        public EditMode ImageEditMode
        {
            get { return _ImageEditMode; }
            set
            {
                _ImageEditMode = value;
                switch (_ImageEditMode)
                {
                    case EditMode.SizeChange:
                        RC_Drager.Visibility = Visibility.Visible;
                        RUC_Drager.Visibility = Visibility.Visible;
                        RD_Drager.Visibility = Visibility.Visible;
                        break;
                }
            }

        }

        public struct WIN32POINT
        {
            public int X;
            public int Y;
        }


        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out WIN32POINT lpPoint);



        float Ratio = 1.0F;

        public ImageEdit()
        {
            InitializeComponent();

            RC_Drager.MouseDown += Drager_Process;
            RUC_Drager.MouseDown += Drager_Process;
            RD_Drager.MouseDown += Drager_Process;

            RD_Drager.MouseUp += DragerEnd_Process;

            tmr.Tick += Tmr_Tick;
            tmr.Interval = 50;

            BitmapImage img = new BitmapImage(new Uri(@"pack://application:,,,/AutoCapturer;component/Resources/Icons/CloseImg.png"));

            CroppedBitmap CropImg = new CroppedBitmap(img, new Int32Rect(20, 20, 30, 30));

            InnerImg.Source = CropImg;
        }


        private void Tmr_Tick(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Control.MouseButtons == f.MouseButtons.Left)
            {

                WIN32POINT NowPosition;
                GetCursorPos(out NowPosition);
                double Width, Height;
                Width = BoardSize.Width - (StartPoint.X - NowPosition.X);
                Height = BoardSize.Height - (StartPoint.Y - NowPosition.Y);
                if (Width < 11) Width = 11; if (Height < 11) Height = 11;
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
                PreviewRect.Opacity = 0.0;
                if (Mode == DragMode.Both || Mode == DragMode.OnlyWidth) { MainGrid.Width = ReservePoint.Width; }
                if (Mode == DragMode.Both || Mode == DragMode.OnlyHeight){MainGrid.Height = ReservePoint.Height; }
                
                
                this.Cursor = null;
                tmr.Stop();
            }
        }

        private void DragerEnd_Process(object sender, MouseButtonEventArgs e)
        {
            Mode = DragMode.Wait;
            StartPoint.X = 0; StartPoint.Y = 0;
            this.Cursor = null;
            tmr.Stop();
        }

        Size BoardSize;
        Size ReservePoint;



        WIN32POINT StartPoint;
        DragMode Mode = DragMode.Wait;

        f.Timer tmr = new f.Timer();

        private void Drager_Process(object sender, MouseButtonEventArgs e)
        {
            this.Cursor = ((Rectangle)sender).Cursor;
            GetCursorPos(out StartPoint);
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
            tmr.Start();
        }

    }
    public enum DragMode
    {
        OnlyWidth, OnlyHeight, Both, Wait
    }

    public enum EditMode
    {
        SizeChange, Select, CropImage
    }
}
