using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCapturer.Observer
{
    class VisAreaObserver
    {
        public event ShowEventHandler ShowStateChanged;
        public delegate void ShowEventHandler(bool IsShowed);

        public void StartObserving()
        {
            ShowStateChanged(true);
        }

    }
}
