using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.IO;
using AutoCapturer.PopUps;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Controls;
using static AutoCapturer.Sounds.NotificationSounds;
using static AutoCapturer.Interop.UnsafeNativeMethods;
using static AutoCapturer.Interop.NativeMethods;
using static AutoCapturer.Globals.Globals;

namespace AutoCapturer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        bool Visibled = true;

        ThicknessAnimation da = new ThicknessAnimation();
        DoubleAnimation OpaAni = new DoubleAnimation();

        SettingWdw sw = new SettingWdw();

        Worker.ImgFromPrtScrWorker ImgWorker = new Worker.ImgFromPrtScrWorker();

        System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();

        public MainWindow()
        {
            InitializeComponent();

            

            MainDispatcher = Dispatcher;
            CurrentSetting = new Setting.Setting();

            CurrentSetting.SettingChange += SettingChange;
            
            this.Topmost = true;
            
            da.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            da.FillBehavior = FillBehavior.Stop;

            OpaAni.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            OpaAni.FillBehavior = FillBehavior.Stop;

            this.Left = 0;  this.Top = 0;

            //tmr.Interval = 100;
            //tmr.Tick += DebugTick;
            //tmr.Start();

            ImgWorker.Find += DetectPrtscr;
            ImgWorker.Work();

            //return;
            this.MouseMove += FrmAppear;
            this.MouseLeave += FrmDisappear;

            FrmDisappear(this,null);

        }

        private void SettingChange()
        {
            PopUps.PopUpWdw wdw = new PopUps.PopUpWdw("설정이 변경되었습니다.", "정상적으로 변경 인식되었습니다.");
            wdw.ShowTime(1000);
        }

        private void DebugTick(object sender, EventArgs e)
        {
            POINT pt;
            GetCursorPos(out pt);
            
            RECT stRect = default(RECT);

            int Ptr = WindowFromPoint(pt).ToInt32();

            GetWindowRect(Ptr, ref stRect);

            TBSpace.Text = "IntPtr : " + Ptr;
        }

        private void FrmAppear(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!Visibled)
            {
                Visibled = true;
                Appear();
            }
        }

        private void FrmDisappear(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (Visibled)
            {
                Visibled = false;
                DisAppear();
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var compositionTarget = PresentationSource.FromVisual(this).CompositionTarget;

            Globals.Globals.RatioX = compositionTarget.TransformToDevice.M11;
            Globals.Globals.RatioY = compositionTarget.TransformToDevice.M22;
        }

        private void ShowStateChanged(bool IsShowed)
        {
            if (IsShowed) Dispatcher.Invoke(new Action(() => { Appear(); }));
            else if (!IsShowed) Dispatcher.Invoke(new Action(() => { DisAppear(); }));
        }

        bool AuCaEnabled = false;
        private void BtnEnAutoSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PopUpWdw pw;
                //DropShadowEffect eff = (DropShadowEffect)MainRect.Effect;
                if (!AuCaEnabled)
                {
                    pw = new PopUpWdw("자동 캡쳐 활성화", "파일로 자동 저장합니다.");
                    PlayNotificationSound(SoundType.AuCaModeOn);

                    //eff.Color = Colors.Red;
                }
                else
                {
                    pw = new PopUps.PopUpWdw("자동 캡쳐 비활성화", "더 이상 저장하지 않습니다.");
                    PlayNotificationSound(SoundType.AuCaModeOff);

                    //eff.Color = Colors.Black;
                }
                pw.ShowTime(1500);
                AuCaEnabled = !AuCaEnabled;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                throw;
            }
            
        }


        public void DisAppear()
        {
            DetectGrid.Width = 1;
            DetectGrid.Height = 1;

            MainGrid.Opacity = 1.0;
            OpaAni.Duration = new Duration(TimeSpan.FromMilliseconds(500));

            Thickness ToThickness = new Thickness(-this.Width, -this.Height, 0, 0);
            Thickness FromThickness = new Thickness(MainGrid.Margin.Left, MainGrid.Margin.Top, 0, 0);

            da.To = ToThickness; MainGrid.Margin = ToThickness;

            OpaAni.To = 0;

            da.From = FromThickness;
            OpaAni.From = 1.0;

            da.AccelerationRatio = 1;
            
            OpaAni.AccelerationRatio = 1;

            MainGrid.BeginAnimation(Grid.MarginProperty, da);

            MainGrid.BeginAnimation(Grid.OpacityProperty, OpaAni);

            MainGrid.Opacity = 0.0;
        }

        void updateSpace(string DriveName)
        {
            SpaceCalculator sc = new SpaceCalculator(DriveName, 2, 99999);

            RemainSpaceRun.Text = sc.RemainPicNumText;
        }

        public void Appear()
        {
            DetectGrid.Width = this.Width;
            DetectGrid.Height = this.Height;

            MainGrid.Opacity = 0.0;

            Thickness ToThickness = new Thickness(0, 0, 0, 0);
            Thickness FromThickness = new Thickness(MainGrid.Margin.Left, MainGrid.Margin.Top, 0, 0);

            da.To = ToThickness; MainGrid.Margin = ToThickness;
            OpaAni.To = 1.0;


            da.From = FromThickness;
            OpaAni.From = 0;

            da.AccelerationRatio = 0;
            OpaAni.AccelerationRatio = 0;


            da.EasingFunction = new CircleEase();

            MainGrid.BeginAnimation(Grid.MarginProperty, da);
            MainGrid.BeginAnimation(Grid.OpacityProperty, OpaAni);
            
            MainGrid.Opacity = 1.0;
        }

        private void BtnAllCapture_Click(object sender, RoutedEventArgs e)
        {
            PlayNotificationSound(SoundType.Captured);

            Windows.ImageEditor ie = new Windows.ImageEditor();

            //ie.Editor.image = CopyScreen();

            //ie.ShowDialog();

            //var filestream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Test{++ctr}.jpg", FileMode.Create);

            //var encoder = new PngBitmapEncoder();

            //encoder.Frames.Add(BitmapFrame.Create(ie.Editor.image));
            //encoder.Save(filestream);

            //filestream.Dispose();
        }

        //int ctr = 0;
        private void DetectPrtscr(object sender, WorkEventArgs e)
        {
            if (AuCaEnabled)
            {
                PlayNotificationSound(SoundType.Captured);

                Windows.ImageEditor ie = new Windows.ImageEditor();

                //ie.Editor.image = (BitmapSource)((Worker.ImageWorkEventArgs)e).Data;

                //ie.ShowDialog();

                //var filestream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Test{++ctr}.jpg", FileMode.Create);

                //var encoder = new PngBitmapEncoder();

                //encoder.Frames.Add(BitmapFrame.Create(ie.Editor.image));
                //encoder.Save(filestream);

                //filestream.Dispose();

            }


            //BitmapEncoder encoder = new PngBitmapEncoder();

            //using (var filestream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Rotate Test.jpg", FileMode.Create))
            //{
            //    encoder.Frames.Add(BitmapFrame.Create(filestream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad));


            //    encoder.Save(filestream);
            //}
        }

        // For Debug
        private static BitmapSource CopyScreen()
        {
            using (var screenBmp = new Bitmap(
                (int)SystemParameters.PrimaryScreenWidth,
                (int)SystemParameters.PrimaryScreenHeight,
                System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (var bmpGraphics = Graphics.FromImage(screenBmp))
                {
                    bmpGraphics.CopyFromScreen(0, 0, 0, 0, screenBmp.Size);
                    return Imaging.CreateBitmapSourceFromHBitmap(
                        screenBmp.GetHbitmap(),
                        IntPtr.Zero,
                        Int32Rect.Empty,
                        BitmapSizeOptions.FromEmptyOptions());
                }
            }
        }



        private void BtnSelCapture_Click(object sender, RoutedEventArgs e)
        {

            // 선택 캡처
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer($"snd1.wav");
            player.Play();
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer($"snd2.wav");
            player.Play();
        }

        private void button_Copy1_Click(object sender, RoutedEventArgs e)
        {
            System.Media.SoundPlayer player = new System.Media.SoundPlayer($"snd3.wav");
            player.Play();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void button_Click_1(object sender, RoutedEventArgs e)
        {
            sw = new SettingWdw();
            sw.ShowDialog();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Windows.ImageEditor ie = new Windows.ImageEditor();

            ie.Editor.image = new BitmapImage(new Uri("/AutoCapturer;component/Resources/Icons/CloseImg.png", UriKind.Relative));
            
            ie.Show();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Hide From Alt+Tab (Tasklist)
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);
        }

        private void BtnSelCapture_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            MessageBox.Show("!");
        }
    }
}
