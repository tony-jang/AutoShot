using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace AutoCapturer.Observer
{
    public struct Win32Point
    {
        public int X;
        public int Y;
    }    
    class VisAreaObserver
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Win32Point lpPoint);


        public event ShowEventHandler ShowStateChanged;
        public delegate void ShowEventHandler(bool IsShowed);


        bool IsVisibled = false;


        public void StartObserving()
        {
            Thread thr = new Thread(() =>
            {
                do
                {
                    Win32Point pos;

                    int PosX = 0;
                    int PosY = 0;

                    GetCursorPos(out pos);
                    Dispatcher.CurrentDispatcher.Invoke(new Action(()=>{
                        double ratioX = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice.M11;
                        double ratioY = PresentationSource.FromVisual(Application.Current.MainWindow).CompositionTarget.TransformToDevice.M22;

                        PosX = (int)(pos.X * ratioX);
                        PosY = (int)(pos.Y * ratioY);
                    }));






                if (!IsVisibled && PosX == 0 && PosY == 0) {
                        ShowStateChanged(true);
                        IsVisibled = true;
                        continue;
                    }


                    if ((PosX >= 0 && PosX <= 300) && (PosY >= 0 && PosY <= 130))
                    {
                        if (IsVisibled)
                        {
                            continue;
                        }
                        
                    }
                    else
                    {
                        if (IsVisibled)
                        {
                            ShowStateChanged(false);
                            IsVisibled = false;
                        }
                    }
                    Thread.Sleep(10);
                } while (true);
            });
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
            
        }

    }
}
