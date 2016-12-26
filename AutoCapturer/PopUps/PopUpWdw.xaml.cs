using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using wf = System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static AutoCapturer.Sounds.NotificationSounds;
using static AutoCapturer.Globals.Globals;

namespace AutoCapturer.PopUps
{
    /// <summary>
    /// PopUpWdw.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PopUpWdw : Window
    {

        public static Stack<PopUpWdw> puw = new Stack<PopUpWdw>();

        
        System.Windows.Point StartPosition;


        public PopUpWdw(string MainMsg, string InnerMsg)
        {
            InitializeComponent();

            puw.Push(this);
            MainTB.Text = MainMsg;
            InnerTB.Text = InnerMsg;
            
            ClickRect.MouseDown += Rect_MD;
            ClickRect.MouseUp += Rect_MU;

            this.Opacity = 0;

            var area = wf.Screen.PrimaryScreen.WorkingArea;

            StartPosition.X = area.Width - this.Width;
            StartPosition.Y = area.Height - this.Height;


            this.Left = StartPosition.X; this.Top = StartPosition.Y;

            this.Topmost = true;
            
        }
        


        /// <summary>
        /// Show 작업과 나머지 모든 창을 모두 닫습니다.
        /// 또한, 지정된 시간만큼 보여줍니다.
        /// </summary>
        public void ShowTime(int VisibleTime)
        {
            while (puw.Count > 0)
            {
                PopUpWdw pw = puw.Pop();

                if (pw != this)
                {
                    pw.Close();
                }
            }
            puw.Push(this);

            this.Show();
            Thread thr = new Thread(() =>
            {
                Dispatcher.Invoke(new Action(delegate {AppearWindow();}));

                Thread.Sleep(VisibleTime);

                Dispatcher.Invoke(new Action(delegate { DisAppearWindow(); }));

            });
            thr.Start();
        }



        private void Rect_MU(object sender, MouseButtonEventArgs e)
        {
            if (ClickBool)
            {
                this.Close();
                ClickBool = false;
            }
            
        }

        bool ClickBool = false;
        private void Rect_MD(object sender, MouseButtonEventArgs e)
        {
            ClickBool = true;
        }

        private void AppearWindow()
        {
            DoubleAnimation TopDa, OpacityDa;
            TopDa = new DoubleAnimation(); OpacityDa = new DoubleAnimation();

            TopDa.From = StartPosition.Y + 50; TopDa.To = StartPosition.Y;
            TopDa.Duration = new Duration(new TimeSpan(5000000));
            TopDa.AccelerationRatio = 1;

            OpacityDa.From = 0.0; OpacityDa.To = 1.0;
            OpacityDa.Duration = new Duration(new TimeSpan(5000000));
            OpacityDa.AccelerationRatio = 1;
            
            BeginAnimation(Window.TopProperty, TopDa);
            BeginAnimation(Window.OpacityProperty, OpacityDa);


            
        }
        private void DisAppearWindow()
        {
            DoubleAnimation TopDa, OpacityDa;
            TopDa = new DoubleAnimation(); OpacityDa = new DoubleAnimation();

            TopDa.From = this.Top; TopDa.To = this.Top += 50;
            TopDa.Duration = new Duration(new TimeSpan(5000000));
            TopDa.AccelerationRatio = 1;

            OpacityDa.From = 1.0; OpacityDa.To = 0.0;
            OpacityDa.Duration = new Duration(new TimeSpan(5000000));
            OpacityDa.AccelerationRatio = 1;
            
            BeginAnimation(Window.TopProperty, TopDa);
            BeginAnimation(Window.OpacityProperty, OpacityDa);
            

        }

        

    }
}
