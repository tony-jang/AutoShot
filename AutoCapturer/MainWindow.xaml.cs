using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.IO;
using AutoCapturer.Windows;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Interop;
using System.Windows.Controls;
using static AutoCapturer.Sounds.NotificationSounds;
using static AutoCapturer.Interop.UnsafeNativeMethods;
using static AutoCapturer.Interop.NativeMethods;
using static AutoCapturer.Globals.Globals;
using System.Windows.Forms;
using System.Windows.Threading;
using AutoCapturer.Worker;

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

        SettingWindow sw = new SettingWindow();

        ImgFromPrtScrWorker ImgWorker = new ImgFromPrtScrWorker();
        StorageSpaceWorker spaceworker = new StorageSpaceWorker();
        ShortCutWorker scworker = new ShortCutWorker();

        
        Timer tmr = new Timer();

        int ExpectSize = 1000;

        public MainWindow()
        {
            InitializeComponent();


            //InitSettingWindow sw = new InitSettingWindow();

            //sw.ShowDialog();

            


            PngBitmapEncoder enc = new PngBitmapEncoder();

            ExpectSize = ImageSourceToBytes(enc,CopyScreen()).Length;

            MainDispatcher = Dispatcher;
            CurrentSetting = new Setting.Setting();
            

            CurrentSetting.Patterns.Add(CurrentSetting.DefaultPattern);

            CurrentSetting.SettingChange += SettingChange;
            this.Topmost = true;
            
            da.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            da.FillBehavior = FillBehavior.Stop;

            OpaAni.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            OpaAni.FillBehavior = FillBehavior.Stop;

            this.Left = 0;  this.Top = 0;

            ImgWorker.Find += DetectPrtscr;
            ImgWorker.Work();

            spaceworker.Find += SpaceChange;
            spaceworker.Work();
            
            this.MouseMove += FrmAppear;
            this.MouseLeave += FrmDisappear;

            FrmDisappear(this,null);

        }

        private void SpaceChange(object sender, WorkEventArgs e)
        {
            StorageSpaceWorkEventArgs ev = (StorageSpaceWorkEventArgs)e;

            

            
            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                SpaceCalculator sc = new SpaceCalculator(ev.driveinfo.Name, ExpectSize, 99999);
                RemainSpaceRun.Text = sc.RemainPicNumText;
                TBSpaceToolTip.RemainPictureCount = sc.RemainPicNum.ToString();
                TBSpaceToolTip.CalcFileSize = Math.Round((double)ExpectSize / 1024 / 1024,4) + "MB";
            }));
        }

        private void SettingChange()
        {
            //PopUpWindow wdw = new PopUpWindow("설정이 변경되었습니다.", "정상적으로 변경 인식되었습니다.");
            //wdw.ShowTime(1000);
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
                PopUpWindow pw;
                //DropShadowEffect eff = (DropShadowEffect)MainRect.Effect;
                if (!AuCaEnabled)
                {
                    pw = new PopUpWindow("자동 캡쳐 활성화", "파일로 자동 저장합니다.");
                    PlayNotificationSound(SoundType.AuCaModeOn);

                    //eff.Color = Colors.Red;
                }
                else
                {
                    pw = new PopUpWindow("자동 캡쳐 비활성화", "더 이상 저장하지 않습니다.");
                    PlayNotificationSound(SoundType.AuCaModeOff);

                    //eff.Color = Colors.Black;
                }
                pw.ShowTime(1500);
                AuCaEnabled = !AuCaEnabled;
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(ex.ToString());
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

        public bool GetImageFromImageEditor(ref BitmapSource image)
        {
            Windows.ImageEditor ie = new Windows.ImageEditor();
            ie.Editor.Originalimage = image;

            Tuple<bool, BitmapImage> data = ie.ShowDialog();

            if (!data.Item1) return false;
            image = ie.GetVisibleEditorImage();

            return true;
        }
        string path;
        private void BtnAllCapture_Click(object sender, RoutedEventArgs e)
        {
            path = Path.Combine(CurrentSetting.DefaultPattern.RealSaveLocation,
                                                         CurrentSetting.DefaultPattern.RealSaveName + ".jpg");
            BitmapSource image = CopyScreen();

            System.Windows.Clipboard.SetData(System.Windows.DataFormats.Bitmap, image);

            PlayNotificationSound(SoundType.Captured);

            if (CurrentSetting.DefaultPattern.OpenEffector)
            {
                if (!GetImageFromImageEditor(ref image)) return;
            }
            var filestream = new FileStream(path, FileMode.Create);

            var encoder = new PngBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(image));
            encoder.Save(filestream);

            filestream.Dispose();


            Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
            {
                PopUpWindow puw = new PopUpWindow("저장 완료됨", "정상적으로 저장이 완료되었습니다.");
                puw.ShowTime(1000);
            }));
        }

        //int ctr = 0;
        private void DetectPrtscr(object sender, WorkEventArgs e)
        {
            path = Path.Combine(CurrentSetting.DefaultPattern.RealSaveLocation,
                                                         CurrentSetting.DefaultPattern.RealSaveName + ".jpg");
            if (AuCaEnabled)
            {
                BitmapSource image = (BitmapSource)((ImageWorkEventArgs)e).Data;

                PlayNotificationSound(SoundType.Captured);

                if (CurrentSetting.DefaultPattern.OpenEffector)
                {
                    if (!GetImageFromImageEditor(ref image)) return;
                }


                var filestream = new FileStream(path, FileMode.Create);

                var encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(filestream);

                filestream.Dispose();

                
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    PopUpWindow puw = new PopUpWindow("저장 완료됨", "정상적으로 저장이 완료되었습니다.");
                    puw.ShowTime(1000);
                }));
                
                

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
        
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Windows.ImageEditor ie = new Windows.ImageEditor();

            ie.Editor.Originalimage = new BitmapImage(new Uri("/AutoCapturer;component/Resources/Icons/CloseImg.png", UriKind.Relative));
            
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
        
        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            
            sw = new SettingWindow();
            sw.ShowDialog();
        }
    }
}
