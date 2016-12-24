using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCapturer.Observer
{
    public struct POINT
    {
        public int X;
        public int Y;
    }    
    class VisAreaObserver
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out POINT lpPoint);


        public event ShowEventHandler ShowStateChanged;
        public delegate void ShowEventHandler(bool IsShowed);


        bool LastState = false;


        public void StartObserving()
        {
            Thread thr = new Thread(() =>
            {
                do
                {
                    POINT pt;
                    GetCursorPos(out pt);

                    if (!LastState && pt.X == 0 && pt.Y == 0) {
                        ShowStateChanged(true);
                        LastState = !LastState;
                        continue;
                    }


                    if ((pt.X >= 0 && pt.X <= 300) && (pt.Y >= 0 && pt.Y <= 130))
                    {
                        if (LastState)
                        {
                            continue;
                        }
                        
                    }
                    else
                    {
                        if (LastState)
                        {
                            ShowStateChanged(false);
                            LastState = !LastState;
                        }
                    }

                } while (true);
            });

            thr.Start();
            
        }

    }
}
