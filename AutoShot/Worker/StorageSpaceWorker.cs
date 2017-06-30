using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AutoShot.Worker
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

                    Thread.Sleep(10);
                } while (true);
            });
        }
    }
    class StorageSpaceWorkEventArgs : WorkEventArgs
    {
        public StorageSpaceWorkEventArgs(long remainpic, DriveInfo di)
        {
            RemainSpace = remainpic;
            Driveinfo = di;
        }
        public long RemainSpace{ get; set; }
        
        public DriveInfo Driveinfo { get; set; }
    }
}
