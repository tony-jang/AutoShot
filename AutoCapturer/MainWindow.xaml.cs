using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Formatting;
using System.Windows.Media.Animation;
using System.IO;
using static AutoCapturer.Sounds.NotificationSounds;
using AutoCapturer.PopUps;
using System.Drawing;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Effects;
using Microsoft.Win32;
using AutoCapturer.Observer;
using System.Windows.Controls;

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

        AutoCapturer.Observer.PrtScrObserver obs = new Observer.PrtScrObserver();

        public MainWindow()
        {
            InitializeComponent();
            
            this.Topmost = true;
            
            
            da.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            da.FillBehavior = FillBehavior.Stop;

            OpaAni.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            OpaAni.FillBehavior = FillBehavior.Stop;

            this.Left = 0;  this.Top = 0;

            this.MouseMove += FrmAppear;
            this.MouseLeave += FrmDisappear;

            obs.DetectPrtScr += Obs_A;
            obs.TestMtd();

            //HttpClient client = new HttpClient();
            //client.BaseAddress = new Uri("http://localhost:52899/");

            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));



            ////TODO : 이 부분에 발생한 오류 처리
            //var str = new string[] { "uutak2000", "uutkk" };

            //MediaTypeFormatter frmter = new JsonMediaTypeFormatter();

            //HttpContent cont = new ObjectContent<string[]>(str, frmter);

            //var resp = client.PostAsync("api/world/Create", cont).Result;



            //// 모든 제품들의 목록.
            //HttpResponseMessage response = client.GetAsync("api/world/name").Result;  // 호출 블록킹!
            //if (response.IsSuccessStatusCode)
            //{
            //    // 응답 본문 파싱. 블록킹!
            //    var products = response.Content.ReadAsAsync<string>().Result;
            //    MessageBox.Show(products);

            //}
            //else
            //{
            //    MessageBox.Show($"{(int)response.StatusCode} ({response.ReasonPhrase})");
            //}
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
            PopUpWdw pw;
            DropShadowEffect eff = (DropShadowEffect)MainRect.Effect;
            if (!AuCaEnabled)
            {
                pw = new PopUpWdw("자동 캡쳐 활성화", "지금부터 캡쳐되는 내용은 자동으로 저장됩니다.");
                PlayNotificationSound(SoundType.AuCaModeOn);

                eff.Color = Colors.Red;
            }
            else
            {
                pw = new PopUps.PopUpWdw("자동 캡쳐 비활성화", "지금부터 캡쳐되는 내용은 클립보드에 저장됩니다.");
                PlayNotificationSound(SoundType.AuCaModeOff);

                eff.Color = Colors.Black;
            }
            pw.ShowTime(1500);
            AuCaEnabled = !AuCaEnabled;
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
            //sw.ShowDialog();
            //Effectors.BaseEffector RtEff = new Effectors.RotateEffector();

            //RtEff.Source = new BitmapImage(new Uri(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\시험 일정표.jpg"));
            //BitmapEncoder encoder = new PngBitmapEncoder();
            //encoder.Frames.Add(BitmapFrame.Create(RtEff.ApplyEffect()));
            //using (var filestream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Rotate Test.jpg", FileMode.Create))
            //{
            //    encoder.Save(filestream);
            //}
        }

        int ctr = 0;
        private void Obs_A(ImageSource Img)
        {
            if (AuCaEnabled)
            {
                PlayNotificationSound(SoundType.Captured);
                var filestream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + $"\\Test{++ctr}.jpg", FileMode.Create);

                var encoder = new PngBitmapEncoder();

                encoder.Frames.Add(BitmapFrame.Create((BitmapSource)Img));
                encoder.Save(filestream);

                filestream.Dispose();

            }


            //BitmapEncoder encoder = new PngBitmapEncoder();

            //using (var filestream = new FileStream(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\Rotate Test.jpg", FileMode.Create))
            //{
            //    encoder.Frames.Add(BitmapFrame.Create(filestream, BitmapCreateOptions.PreservePixelFormat, BitmapCacheOption.OnLoad));


            //    encoder.Save(filestream);
            //}
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

    }
}
