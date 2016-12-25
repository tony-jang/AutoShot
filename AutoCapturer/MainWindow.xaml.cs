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

namespace AutoCapturer
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        DoubleAnimation da = new DoubleAnimation();
        DoubleAnimation da2 = new DoubleAnimation();
        DoubleAnimation OpaAni = new DoubleAnimation();

        SettingWdw sw = new SettingWdw();

        AutoCapturer.Observer.PrtScrObserver obs = new Observer.PrtScrObserver();
        AutoCapturer.Observer.VisAreaObserver vis = new Observer.VisAreaObserver();

        public MainWindow()
        {
            InitializeComponent();
            
            this.Topmost = true;

            vis.ShowStateChanged += ShowStateChanged;

            
            da.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            da.FillBehavior = FillBehavior.Stop;

            da2.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            da2.FillBehavior = FillBehavior.Stop;

            OpaAni.Duration = new Duration(TimeSpan.FromMilliseconds(800));
            OpaAni.FillBehavior = FillBehavior.Stop;

            


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
            pw.ShowTime(5000);
            AuCaEnabled = !AuCaEnabled;
        }


        public void DisAppear()
        {
            this.Opacity = 1.0;
            OpaAni.Duration = new Duration(TimeSpan.FromMilliseconds(500));


            da.To = -this.Width;
            da2.To = -this.Height;
            OpaAni.To = 0;

            da.From = 0;
            da2.From = 0;
            OpaAni.From = 1.0;

            da.AccelerationRatio = 1;
            da2.AccelerationRatio = 1;
            OpaAni.AccelerationRatio = 1;

            this.BeginAnimation(Window.LeftProperty, da);
            this.BeginAnimation(Window.TopProperty, da2);
            this.BeginAnimation(Window.OpacityProperty, OpaAni);

            this.Opacity = 0.0;
        }

        public void Appear()
        {
            this.Opacity = 0.0;
            da.To = 0;
            da2.To = 0;
            OpaAni.To = 1.0;


            da.From = -this.Width;
            da2.From = -this.Height;
            OpaAni.From = 0;
            
            da.AccelerationRatio = 0;
            da2.AccelerationRatio = 0;
            OpaAni.AccelerationRatio = 0;


            da.EasingFunction = new CircleEase();
            da2.EasingFunction = new CircleEase();


            this.BeginAnimation(Window.LeftProperty, da);
            this.BeginAnimation(Window.TopProperty, da2);
            this.BeginAnimation(Window.OpacityProperty, OpaAni);

            this.Opacity = 1.0;
        }

        private void BtnAllCapture_Click(object sender, RoutedEventArgs e)
        {
            

            obs.DetectPrtScr += Obs_A;
            obs.TestMtd();
            vis.StartObserving();
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
