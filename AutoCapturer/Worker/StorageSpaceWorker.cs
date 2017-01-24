using System;
using System.Collections.Generic;
using System.IO;
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

        long remainingspace = -1;
        public StorageSpaceWorker()
        {
            thr = new Thread(() =>
            {
                do
                {
                    DriveInfo LookingDrive = null;
                    foreach (DriveInfo di in DriveInfo.GetDrives())
                        if (di.Name == LookDrive) LookingDrive = di;
                    
                    
                    if (remainingspace != LookingDrive.TotalFreeSpace)
                        OnFind(new StorageSpaceWorkEventArgs(remainingspace, LookingDrive));
                    remainingspace = LookingDrive.TotalFreeSpace;

                    Thread.Sleep(1);
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
        public StorageSpaceWorkEventArgs(long remainpic,DriveInfo di)
        {
            RemainSpace = remainpic;
            _driveinfo = di;
        }
        private long _RemainSpace;
        public long RemainSpace
        {
            get { return _RemainSpace; }
            set { _RemainSpace = value; }
        }

        private DriveInfo _driveinfo;
        public DriveInfo driveinfo
        {
            get { return _driveinfo; }
            set { _driveinfo = value; }
        }
    }
}
