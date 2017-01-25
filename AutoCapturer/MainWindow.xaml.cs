using System;
using System.Windows;
using System.Windows.Media.Animation;
using System.IO;
using AutoCapturer.Windows;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using static AutoCapturer.Sounds.NotificationSounds;
using static AutoCapturer.Interop.UnsafeNativeMethods;
using static AutoCapturer.Interop.NativeMethods;
using static AutoCapturer.Globals.Globals;
using static AutoCapturer.Globals.ImageUtilities;
using f = System.Windows.Forms;
using s = System.Windows.Shapes;
using w = System.Windows;
using d = System.Drawing;
using System.Windows.Threading;
using AutoCapturer.Worker;
using AutoCapturer.Globals;
using System.Threading;
using AutoCapturer.Setting;
using System.Windows.Shapes;
using AutoCapturer.UserControls;
using System.Text;
using System.Net;
using Microsoft.Win32;

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
        ImgFromURLWorker urlworker = new ImgFromURLWorker();

        int ExpectSize = 1000;

        public MainWindow()
        {
            InitializeComponent();
            //try
            //{

            PngBitmapEncoder enc = new PngBitmapEncoder();

            ExpectSize = ImageSourceToBytes(enc, CopyScreen()).Length;

            MainDispatcher = Dispatcher;

            CurrentSetting = new SettingReader().ReadSetting();
            if (CurrentSetting == null)
                CurrentSetting = (new InitSettingWindow()).ShowDialog();

            CurrentSetting.AddHandler();
            CurrentSetting.SettingChangeEvent += SettingChange;

            if (!CurrentSetting.TutorialProgress)
            {
                if (MsgBox("Auto Capturer 도움말을 엽니다. 계속하시겠습니까?", "도움말", MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
                {
                    var hw = new HelpWindow();
                    hw.ShowDialog();
                }


                CurrentSetting.TutorialProgress = true;
            }


            this.Topmost = true;

            da.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            da.FillBehavior = FillBehavior.Stop;

            OpaAni.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            OpaAni.FillBehavior = FillBehavior.Stop;

            this.Left = 0; this.Top = 0;

            ImgWorker.Find += DetectPrtscr;
            ImgWorker.Work();

            spaceworker.Find += SpaceChange;
            spaceworker.Work();

            try
            {
                if (IsImage(Clipboard.GetText())) Clipboard.Clear();
            }
            catch { }


            urlworker.Find += DetectURL;
            urlworker.Work();

            scworker.Keys.Add(CurrentSetting.AllCaptureKey);
            scworker.Keys.Add(CurrentSetting.AutoCaptureKey);
            scworker.Keys.Add(CurrentSetting.OpenSettingKey);
            scworker.Keys.Add(CurrentSetting.ChangeEditorModeKey);
            scworker.Keys.Add(CurrentSetting.SelectCaptureKey);

            scworker.Find += DetectShortCut;
            scworker.Work();

            this.MouseMove += FrmAppear;
            this.MouseLeave += FrmDisappear;

            FrmDisappear(this, null);

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}


        }


        private void DetectURL(object sender, WorkEventArgs e)
        {
            try
            {
                ImageSizeEventArgs ev = (ImageSizeEventArgs)e;

                if (CurrentSetting.ImageFromURLSave == HowtoSaveGetPicture.NoUse) return;

                BitmapImage image = new BitmapImage();
                image.BeginInit();
                image.UriSource = ev.Img.UriSource;
                image.DecodePixelHeight = (int)ev.ImageSize.Height;
                image.DecodePixelWidth = (int)ev.ImageSize.Width;
                image.EndInit();

                if (CurrentSetting.ImageFromURLSave == HowtoSaveGetPicture.PatternSave)
                {
                    SaveRequest(image);
                }
                else if (CurrentSetting.ImageFromURLSave == HowtoSaveGetPicture.Ask)
                {
                    if (MsgBox("사진이 포함된 URL을 감지했습니다. 저장할까요?", "사진 저장 확인", MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
                    {
                        SaveRequest(image);
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
            PopUpWindow pw;
            if (CurrentSetting.DefaultPattern.OpenEffector)
            {
                pw = new PopUpWindow("이미지 에디터 모드 변경", "캡쳐 후 에디터를 열지 않게 변경되었습니다.");
            }
            else
            {
                pw = new PopUpWindow("이미지 에디터 모드 변경", "캡쳐 후 에디터가 열리게 변경되었습니다.");
            }
            pw.ShowTime(CurrentSetting.PopupCountSecond * 1000);
            
            CurrentSetting.DefaultPattern.OpenEffector = !CurrentSetting.DefaultPattern.OpenEffector;
        }

        private void SpaceChange(object sender, WorkEventArgs e)
        {
            try
            {
                StorageSpaceWorkEventArgs ev = (StorageSpaceWorkEventArgs)e;

                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    SpaceCalculator sc = new SpaceCalculator(ev.driveinfo.Name, ExpectSize, 99999);
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

        private void SettingChange()
        {
            DetectGrid.Width = CurrentSetting.RecoWidth;
            DetectGrid.Height = CurrentSetting.RecoHeight;

            RegistryKey rkey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");

            if (CurrentSetting.IsStartupProgram)
                rkey.SetValue("AutoCapturer", $"\"{System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName}\" -bystartup");
            else
                rkey.DeleteValue("AutoCapturer", false);

            SettingWriter sw = new SettingWriter((Setting.Setting)CurrentSetting.Clone());
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
                if (!AuCaEnabled)
                {
                    pw = new PopUpWindow("자동 캡쳐 활성화", "파일로 자동 저장합니다.");
                    if (CurrentSetting.AutoCaptureEnableSelection == Setting.AuCaEnableSelection.SoundPlay ||
                        CurrentSetting.AutoCaptureEnableSelection == Setting.AuCaEnableSelection.SoundAndPopUp)
                        PlayNotificationSound(SoundType.AuCaModeOn);

                }
                else
                {
                    pw = new PopUpWindow("자동 캡쳐 비활성화", "더 이상 저장하지 않습니다.");
                    if (CurrentSetting.AutoCaptureEnableSelection == Setting.AuCaEnableSelection.SoundPlay ||
                        CurrentSetting.AutoCaptureEnableSelection == Setting.AuCaEnableSelection.SoundAndPopUp)
                        PlayNotificationSound(SoundType.AuCaModeOff);

                }
                if (CurrentSetting.AutoCaptureEnableSelection == Setting.AuCaEnableSelection.OpenPopup ||
                        CurrentSetting.AutoCaptureEnableSelection == Setting.AuCaEnableSelection.SoundAndPopUp)
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
            ImageEditor ie = new ImageEditor();
            ie.Editor.Originalimage = image;

            Tuple<bool, BitmapImage> data = ie.ShowDialog();

            if (!data.Item1) return false;
            image = ie.GetVisibleEditorImage();

            return true;
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

        public void SaveRequest(BitmapSource image)
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
                if (CurrentSetting.DefaultPattern.OpenEffector)
                {
                    if (!GetImageFromImageEditor(ref image)) return;
                }

                var filestream = new FileStream(path, FileMode.Create);

                var encoder = new PngBitmapEncoder();


                encoder.Frames.Add(BitmapFrame.Create(image));
                encoder.Save(filestream);

                encoder = new PngBitmapEncoder();

                ExpectSize = ImageSourceToBytes(encoder, image).Length;

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
        d.Rectangle FullBound = d.Rectangle.Empty;
        BlankRect blnkrect;

        bool CaptureCompFlag = false;

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


                // 선택 캡처

                foreach (var scr in f.Screen.AllScreens)
                {
                    FullBound = d.Rectangle.Union(FullBound, scr.Bounds);
                }

                tmr.Interval = 1;
                tmr.Tick += Dragging;

                grid = new Grid();

                PlayNotificationSound(SoundType.Captured);


                blnkrect = new BlankRect();

                blnkrect.Fill = (w.Media.Brush)(new BrushConverter()).ConvertFromString("#7FFFFFFF");

                grid.Children.Add(blnkrect);

                dragGrid = new Border();
                grid.Children.Add(dragGrid);
                dragGrid.HorizontalAlignment = HorizontalAlignment.Left;
                dragGrid.VerticalAlignment = VerticalAlignment.Top;
                dragGrid.BorderBrush = w.Media.Brushes.Red;
                dragGrid.BorderThickness = new Thickness(1);
                dragGrid.Width = 100;
                dragGrid.Height = 100;
                dragGrid.Visibility = Visibility.Hidden;

                //TODO : 커서 변환 완성시키기
                blnkrect.MouseDown += DragStart;
                //blnkrect.Cursor = CursorHelper.CreateCursor(
                //    BitmapImage2Bitmap(new BitmapImage(new Uri("/AutoCapturer;component/Resources/Icons/SmallIcons/DeskIcon.png", UriKind.Relative))),0,0);

                Wdw = new Window();
                Wdw.AllowsTransparency = true;
                Wdw.Topmost = true;
                Wdw.ResizeMode = ResizeMode.NoResize;
                Wdw.Left = FullBound.Left;
                Wdw.Top = FullBound.Top;
                Wdw.Width = FullBound.Width;
                Wdw.Height = FullBound.Height;
                Wdw.WindowStyle = WindowStyle.None;

                Wdw.MouseDown += DragStart;

                Wdw.Background = new ImageBrush(CopyScreen());

                Wdw.Content = grid;
                Wdw.Loaded += WdwInit;
                Wdw.ShowDialog();


                if (!CaptureCompFlag) return;

                Dispatcher.Invoke(new Action(() =>
                {
                    this.IsEnabled = true;
                }));

                
                BitmapSource image = new CroppedBitmap((BitmapSource)((ImageBrush)Wdw.Background).ImageSource,
                    new Int32Rect(blnkrect.Rect.X, blnkrect.Rect.Y, blnkrect.Rect.Width, blnkrect.Rect.Height));

                SaveRequest(image);



            });


            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();

        }

        private void WdwInit(object sender, RoutedEventArgs e)
        {
            blnkrect.Rect = new d.Rectangle(0, 0, 0, 0);
            blnkrect.UpdateGeometry();
        }

        private void Dragging(object sender, EventArgs e)
        {
            POINT NowLoc;
            GetCursorPos(out NowLoc);

            if (StartLoc.X < NowLoc.X)
            {
                dragGrid.Margin = GetMargin(dragGrid.Margin, ThickPos.Left, StartLoc.X - FullBound.Left, FullBound.Width);
                dragGrid.Width = (NowLoc.X - StartLoc.X);
            }
            else
            {
                dragGrid.Margin = GetMargin(dragGrid.Margin, ThickPos.Left, NowLoc.X - FullBound.Left, FullBound.Width);
                dragGrid.Width = (StartLoc.X - NowLoc.X);
            }
            if (StartLoc.Y > NowLoc.Y)
            {
                dragGrid.Margin = GetMargin(dragGrid.Margin, ThickPos.Top, NowLoc.Y - FullBound.Top, FullBound.Width);
                dragGrid.Height = (StartLoc.Y - NowLoc.Y);
            }
            else
            {
                dragGrid.Margin = GetMargin(dragGrid.Margin, ThickPos.Top, StartLoc.Y - FullBound.Top, FullBound.Width);
                dragGrid.Height = (NowLoc.Y - StartLoc.Y);
            }


            blnkrect.Rect = new d.Rectangle((int)dragGrid.Margin.Left, (int)dragGrid.Margin.Top, (int)dragGrid.Width, (int)dragGrid.Height);


            if (Mouse.LeftButton == MouseButtonState.Released)
            {
                tmr.Stop();

                if (dragGrid.Width == 0 || dragGrid.Height == 0)
                {
                    dragGrid.Visibility = Visibility.Hidden;
                    dragGrid.Width = 0;
                    dragGrid.Height = 0;
                }
                else
                {
                    CaptureCompFlag = true;
                    Wdw.Close();
                }


            }
        }

        POINT StartLoc;
        f.Timer tmr = new f.Timer();
        private void DragStart(object sender, MouseButtonEventArgs e)
        {
            POINT pt;
            GetCursorPos(out pt);
            dragGrid.Visibility = Visibility.Visible;
            StartLoc = pt;
            tmr.Start();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Hide From Alt+Tab (Tasklist)
            WindowInteropHelper wndHelper = new WindowInteropHelper(this);

            int exStyle = (int)GetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE);

            exStyle |= (int)ExtendedWindowStyles.WS_EX_TOOLWINDOW;
            SetWindowLong(wndHelper.Handle, (int)GetWindowLongFields.GWL_EXSTYLE, (IntPtr)exStyle);

            this.Topmost = true;
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            scworker.IsUsed = false;
            sw = new SettingWindow();
            sw.ShowDialog();
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
