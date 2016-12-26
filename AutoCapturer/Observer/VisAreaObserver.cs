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

                    PosX = (int)(pos.X * Globals.Globals.RatioX);
                    PosY = (int)(pos.Y * Globals.Globals.RatioY);


                    Thread.Sleep(10);
                } while (true);
            });
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
            
        }

    }
}
