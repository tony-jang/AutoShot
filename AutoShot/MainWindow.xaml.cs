using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Threading;

using AutoShot.Windows;
using AutoShot.Worker;
using AutoShot.Globals;
using AutoShot.Setting;
using AutoShot.UserControls;
using AutoShot.Utils;

using static AutoShot.Sounds.NotificationSounds;
using static AutoShot.Interop.UnsafeNativeMethods;
using static AutoShot.Globals.Globals;
using static AutoShot.Globals.ImageUtilities;

using f = System.Windows.Forms;
using w = System.Windows;

using Microsoft.Win32;


namespace AutoShot
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

        PrintScreenWorker ImgWorker = new PrintScreenWorker();
        StorageSpaceWorker spaceworker = new StorageSpaceWorker();
        ShortCutWorker scworker = new ShortCutWorker();
        ImageWorker urlworker = new ImageWorker();


        private f.NotifyIcon notify;

        int ExpectSize = 1000;

        public MainWindow()
        {
            InitializeComponent();
            
            try
            {

                PngBitmapEncoder enc = new PngBitmapEncoder();
                
                ExpectSize = ImageSourceToBytes(enc, CopyScreen()).Length;

                MainDispatcher = Dispatcher;
                CurrentSetting = new SettingReader().ReadSetting();


                SettingLocation = Registry.GetValue(Registry.CurrentUser.ToString() + "\\AutoCapturer", "SettingLocation", "NotFound").ToString();

                if (CurrentSetting == null)
                {
                    CurrentSetting = (new InitSettingWindow()).ShowDialog();
                }

                CurrentSetting.AddHandler();
                if (!CurrentSetting.TutorialProgress)
                {
                    if (MsgBox("Auto Capturer 도움말을 엽니다. 계속하시겠습니까?", "도움말", MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
                    {
                        var hw = new HelpWindow();
                        hw.ShowDialog();
                    }


                    CurrentSetting.TutorialProgress = true;
                }

                this.Deactivated += thisDeactivated;
                this.Activated += thisActivated;

                SystemEvents.SessionEnding += ComputerShutdown;

                this.Topmost = true;

                da.Duration = new Duration(TimeSpan.FromMilliseconds(800));
                da.FillBehavior = FillBehavior.Stop;

                OpaAni.Duration = new Duration(TimeSpan.FromMilliseconds(800));
                OpaAni.FillBehavior = FillBehavior.Stop;

                this.Left = 0; this.Top = 0;

                ImgWorker.Find += DetectPrtscr;

                spaceworker.Find += SpaceChange;

                try
                {
                    if (IsImage(Clipboard.GetText())) Clipboard.Clear();
                }
                catch { }


                urlworker.Find += DetectURL;

                scworker.Keys.Add(CurrentSetting.AllCaptureKey);
                scworker.Keys.Add(CurrentSetting.AutoCaptureKey);
                scworker.Keys.Add(CurrentSetting.OpenSettingKey);
                scworker.Keys.Add(CurrentSetting.ChangeEditorModeKey);
                scworker.Keys.Add(CurrentSetting.SelectCaptureKey);

                scworker.Find += DetectShortCut;

                this.MouseMove += FrmAppear;
                this.MouseLeave += FrmDisappear;

                FrmDisappear(this, null);


                f.ContextMenu menu = new f.ContextMenu();

                f.MenuItem itm1 = new f.MenuItem();
                f.MenuItem itm2 = new f.MenuItem();
                f.MenuItem itm3 = new f.MenuItem();
                menu.MenuItems.Add(itm1);
                menu.MenuItems.Add(itm2);
                menu.MenuItems.Add(itm3);

                itm1.Index = 0;
                itm1.Text = "프로그램 종료";
                itm1.Click += delegate (object s, EventArgs e)
                {
                    this.Close();
                };

                itm2.Index = 0;
                itm2.Text = "도움말 보기";
                itm2.Click += delegate (object s, EventArgs e)
                {
                    HelpButt_Click(null, null);
                };

                itm3.Index = 0;
                itm3.Text = "설정";
                itm3.Click += delegate (object s, EventArgs e)
                {
                    PathButton_Click(null, null);
                };

                notify = new f.NotifyIcon();
                notify.ContextMenu = menu;
                notify.Text = "AutoCapturer 동작 중";
                notify.Visible = true;
                notify.Icon = Properties.Resources.AucaIcon;

                notify.BalloonTipTitle = "AutoCapturer 실행 알림";
                notify.BalloonTipText = "AutoCapturer가 실행되었습니다.";
                notify.ShowBalloonTip(1000);
                
                ClipboardMonitor.Start();
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
            }

            sw.Close();
        }

        private void ComputerShutdown(object sender, SessionEndingEventArgs e)
        {
            Environment.Exit(0);
        }

        private void thisActivated(object sender, EventArgs e)
        {
            this.Topmost = false;
            this.Topmost = true;
        }

        private void thisDeactivated(object sender, EventArgs e)
        {
            this.Topmost = true;
        }

        private void DetectURL(object sender, WorkEventArgs e)
        {
            try
            {
                ImageSizeEventArgs ev = (ImageSizeEventArgs)e;

                if (CurrentSetting.ImageFromURLSave == ImageSaveMode.NoUse) return;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = ev.Img.UriSource;
                image.DecodePixelHeight = (int)ev.ImageSize.Height;
                image.DecodePixelWidth = (int)ev.ImageSize.Width;
                image.EndInit();

                if (CurrentSetting.ImageFromURLSave == ImageSaveMode.PatternSave)
                {
                    SaveRequest(image, false);
                }
                else if (CurrentSetting.ImageFromURLSave == ImageSaveMode.Ask)
                {
                    if (MsgBox("사진이 포함된 URL을 감지했습니다. 저장할까요?", "사진 저장 확인", MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
                    {
                        SaveRequest(image, false);
                    }
                }
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
            }
        }

        private void DetectShortCut(object sender, WorkEventArgs e)
        {
            try
            {
                ShortCutWorkEventArgs ev = (ShortCutWorkEventArgs)e;

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    switch (ev.Data.SeparateKey)
                    {
                        case "AutoCapture":
                            BtnEnAutoSave_Click(this, null);
                            break;
                        case "AllCapture":
                            BtnAllCapture_Click(this, null);
                            break;
                        case "OpenSetting":
                            PathButton_Click(this, null);
                            break;
                        case "SelCapture":
                            BtnSelCapture_Click(this, null);
                            break;
                        case "ChangeEditorMode":
                            ChangeEditorMode();
                            break;
                    }
                }));
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
            }

        }

        public void ChangeEditorMode()
        {
            try
            {
                PopUpWindow pw;
                if (CurrentSetting.DefaultPattern.OpenEffector)
                {
                    pw = new PopUpWindow("이미지 에디터 모드 변경", "캡처 후 에디터를 열지 않게 변경했습니다.");
                }
                else
                {
                    pw = new PopUpWindow("이미지 에디터 모드 변경", "캡처 후 에디터가 열리게 변경했습니다.");
                }
                pw.ShowTime(CurrentSetting.PopupCountSecond * 1000);

                CurrentSetting.DefaultPattern.OpenEffector = !CurrentSetting.DefaultPattern.OpenEffector;
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
                throw;
            }
            
        }

        private void SpaceChange(object sender, WorkEventArgs e)
        {
            try
            {
                StorageSpaceWorkEventArgs ev = (StorageSpaceWorkEventArgs)e;

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    SpaceCalculator sc = new SpaceCalculator(ev.Driveinfo.Name, ExpectSize, 99999);
                    RemainSpaceRun.Text = sc.RemainPicNumText;
                    TBSpaceToolTip.RemainPictureCount = sc.RemainPicNum.ToString();
                    TBSpaceToolTip.CalcFileSize = Math.Round((double)ExpectSize / 1024 / 1024, 4) + "MB";
                }));
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
            }

        }

        private void FrmAppear(object sender, MouseEventArgs e)
        {
            if (!Visibled)
            {
                Visibled = true;
                Appear();
            }
        }
        private void FrmDisappear(object sender, w.Input.MouseEventArgs e)
        {
            if (Visibled)
            {
                Visibled = false;
                DisAppear();
            }
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            try
            {
                base.OnSourceInitialized(e);

                var compositionTarget = PresentationSource.FromVisual(this).CompositionTarget;

                RatioX = compositionTarget.TransformToDevice.M11;
                RatioY = compositionTarget.TransformToDevice.M22;
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
                throw;
            }
            
        }

        private void ShowStateChanged(bool IsShowed)
        {
            try
            {
                if (IsShowed) Dispatcher.Invoke(new Action(() => { Appear(); }));
                else if (!IsShowed) Dispatcher.Invoke(new Action(() => { DisAppear(); }));
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
                throw;
            }
            
        }

        bool AuCaEnabled = false;
        private void BtnEnAutoSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PopUpWindow pw;
                if (!AuCaEnabled)
                {
                    pw = new PopUpWindow("자동 캡처 활성화", "파일로 자동 저장합니다.");
                    if (CurrentSetting.AutoCaptureEnableSelection == Setting.CaptureNotifionMode.SoundPlay ||
                        CurrentSetting.AutoCaptureEnableSelection == Setting.CaptureNotifionMode.SoundAndPopUp)
                        PlayNotificationSound(SoundType.AuCaModeOn);

                }
                else
                {
                    pw = new PopUpWindow("자동 캡처 비활성화", "더 이상 저장하지 않습니다.");
                    if (CurrentSetting.AutoCaptureEnableSelection == Setting.CaptureNotifionMode.SoundPlay ||
                        CurrentSetting.AutoCaptureEnableSelection == Setting.CaptureNotifionMode.SoundAndPopUp)
                        PlayNotificationSound(SoundType.AuCaModeOff);

                }
                if (CurrentSetting.AutoCaptureEnableSelection == Setting.CaptureNotifionMode.OpenPopup ||
                        CurrentSetting.AutoCaptureEnableSelection == Setting.CaptureNotifionMode.SoundAndPopUp)
                    pw.ShowTime(CurrentSetting.PopupCountSecond * 1000);
                AuCaEnabled = !AuCaEnabled;
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString(), "예외 발생!");
            }

        }


        public void DisAppear()
        {
            DetectGrid.Width = CurrentSetting.RecoWidth;
            DetectGrid.Height = CurrentSetting.RecoHeight;

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


        void updateSpace(string DriveName)
        {
            SpaceCalculator sc = new SpaceCalculator(DriveName, 2, 99999);

            RemainSpaceRun.Text = sc.RemainPicNumText;
        }

        public bool GetImageFromImageEditor(ref BitmapSource image)
        {
            try
            {
                ImageEditor ie = new ImageEditor();
                ie.Editor.Originalimage = image;

                Tuple<bool, BitmapImage> data = ie.ShowDialog();

                if (!data.Item1) return false;
                image = ie.GetVisibleEditorImage();

                return true;
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
                throw;
            }
            
        }
        string path;
        private void BtnAllCapture_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Thread thr = new Thread(() =>
                {
                    Dispatcher.Invoke(new Action(() => { this.IsEnabled = false; DisAppear(); }));

                    if (Visibled) Thread.Sleep(500);

                    var wdw = new CountDownWindow(CurrentSetting.AllCaptureCountDown);

                    wdw.ShowDialog();


                    path = System.IO.Path.Combine(CurrentSetting.DefaultPattern.RealSaveLocation,
                                                                 CurrentSetting.DefaultPattern.RealSaveName + ".jpg");
                    BitmapSource image = CopyScreen();

                    Clipboard.SetData(DataFormats.Bitmap, image);

                    PlayNotificationSound(SoundType.Captured);

                    SaveRequest(image);

                    Dispatcher.Invoke(new Action(() => { this.IsEnabled = true; }));
                });

                thr.SetApartmentState(ApartmentState.STA);
                thr.Start();
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
            }
        }

        public void SaveRequest(BitmapSource image, bool UpdateStoragy = true)
        {
            try
            {
                path = System.IO.Path.Combine(CurrentSetting.DefaultPattern.RealSaveLocation,
                                                         CurrentSetting.DefaultPattern.RealSaveName + ".jpg");
                if (File.Exists(path) && !CurrentSetting.DefaultPattern.OverWrite)
                {
                    bool flag = false;
                    int Counter = 1;
                    while (!flag)
                    {
                        string tmp;
                        tmp = System.IO.Path.Combine(CurrentSetting.DefaultPattern.RealSaveLocation,
                                                         CurrentSetting.DefaultPattern.RealSaveName + $"({Counter})" + ".jpg");
                        Counter++;
                        if (File.Exists(tmp)) continue;
                        else if (!File.Exists(tmp)) { path = tmp; break; }
                    }
                }

                if (CurrentSetting.DefaultPattern.SaveImmediately)
                {
                    Clipboard.SetData(DataFormats.Bitmap, image);
                }

                if (CurrentSetting.DefaultPattern.OpenEffector)
                {
                    if (!GetImageFromImageEditor(ref image)) return;
                }

                var filestream = new FileStream(path, FileMode.Create);

                var encoder = new PngBitmapEncoder();


                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(filestream);

                encoder = new PngBitmapEncoder();

                if (UpdateStoragy) ExpectSize = ImageSourceToBytes(encoder, image).Length;

                filestream.Dispose();


                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    PopUpWindow puw = new PopUpWindow("저장 완료됨", "정상적으로 저장이 완료되었습니다.");
                    puw.ShowTime(1000);
                }));
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
            }
        }

        private void DetectPrtscr(object sender, WorkEventArgs e)
        {
            try
            {
                if (AuCaEnabled)
                {
                    BitmapSource image = (BitmapSource)((ImageWorkEventArgs)e).Data;

                    PlayNotificationSound(SoundType.Captured);

                    SaveRequest(image);
                }
            }
            catch (Exception ex)
            {
                MsgBox(ex.ToString());
            }
        }
        Window Wdw;
        Grid grid;
        Border dragGrid;
        
        BlankRect blnkrect;

        

        private void BtnSelCapture_Click(object sender, RoutedEventArgs e)
        {
            Thread thr = new Thread(() =>
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    this.IsEnabled = false;
                    FrmDisappear(this, null);
                }));

                Thread.Sleep(500);

                // CropWindow
                
                Dispatcher.Invoke(new Action(() =>
                {
                    this.IsEnabled = true;
                }));


                //if (!CaptureCompFlag) return;

                BitmapSource image = new CroppedBitmap((BitmapSource)((ImageBrush)Wdw.Background).ImageSource,
                    new Int32Rect(blnkrect.Rect.X, blnkrect.Rect.Y, blnkrect.Rect.Width, blnkrect.Rect.Height));

                SaveRequest(image, false);
            });
            
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
        }
 
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Hide From Alt+Tab (Tasklist)
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

            this.Topmost = true;
        }

        // 세팅 창 오픈
        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            scworker.IsUsed = false;
            sw = new SettingWindow();
            sw.ShowDialog();

            DetectGrid.Width = CurrentSetting.RecoWidth;
            DetectGrid.Height = CurrentSetting.RecoHeight;

            RegistryKey rkey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");

            if (CurrentSetting.IsStartupProgram)
                rkey.SetValue("AutoCapturer", $"\"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName}\" -bystartup");
            else
                rkey.DeleteValue("AutoCapturer", false);

            SettingWriter settingw = new SettingWriter((Setting.Setting)CurrentSetting.Clone());

            scworker.IsUsed = true;
        }

        private void HelpButt_Click(object sender, RoutedEventArgs e)
        {
            var hw = new HelpWindow();

            hw.ShowDialog();
        }

        private void CrossButt_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
