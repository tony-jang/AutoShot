using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoShot.Windows
{
    /// <summary>
    /// CountDownWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class CountDownWindow : Window
    {
        public CountDownWindow(int countdownsec)
        {
            InitializeComponent();
            CountDownInt = countdownsec;
            this.Activate();
            this.Topmost = true;
        }

        int CountDownInt;

        public new void ShowDialog()
        {

            var thr = new Thread(() =>
            {
                for (int i = CountDownInt; i >= 1; i--)
                {
                    Dispatcher.Invoke(new Action(() =>
                    {
                        cdTB.Text = i.ToString();
                    }));
                    Thread.Sleep(1000);
                }
                Dispatcher.Invoke(new Action(() => { Close(); }));
                
            });

            thr.Start();
            base.ShowDialog();



        }
    }
}
