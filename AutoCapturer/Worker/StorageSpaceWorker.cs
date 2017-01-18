using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoCapturer.Worker
{
    class StorageSpaceWorker : BaseWorker
    {
        Thread thr;
        // TODO : LookDrive 변경 가능히 만들기 + 용량 계산용 int 하나 더 추가
        string LookDrive = "C:\\";
        public StorageSpaceWorker()
        {
            thr = new Thread(() =>
            {
                do
                {
                    //TODO:드라이브 공간이 변경되면 OnFind 메소드로 알림
                } while (true);
            });
        }
        public override void Rest()
        {
            Stop();
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
    class StorageSpaceWorkEventArgs : WorkEventArgs
    {
        int RemainPicture;
    }
}
