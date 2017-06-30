using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static AutoShot.Globals.Globals;

namespace AutoShot
{
    /// <summary>
    /// App.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class App : Application
    {
        Mutex _mutex = null;

        protected override void OnStartup(StartupEventArgs e)
        {
            string mutexName = "AutoCapturer";
            bool isCreatedNew = false;
            try
            {
                _mutex = new Mutex(true, mutexName, out isCreatedNew);


                if (isCreatedNew)
                {
                    base.OnStartup(e);
                }
                else
                {
                    MsgBox("이미 실행되어 있습니다!", "오류!");
                    Application.Current.Shutdown();
                }


            }
            catch (Exception ex)
            {
                MsgBox(ex.Message + "\n\n" + ex.StackTrace + "\n\n" + "애플리케이션 존재..", "예외 발생!");
                Application.Current.Shutdown();
            }
        }
    }
}
