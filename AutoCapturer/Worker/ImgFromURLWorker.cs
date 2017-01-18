using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCapturer.Worker
{
    class ImgFromURLWorker : BaseWorker
    {
        Thread thr;

        public ImgFromURLWorker()
        {
            thr = new Thread(() =>
            {
                do
                {
                    
                    OnFind(new WorkEventArgs());
                } while (true);
            });
        }

        public override void Rest()
        {
            this.Stop();
        }

        public override void Stop()
        {
            thr.Abort();
        }

        public override void Work()
        {
            thr.Start();
        }
    }

}
