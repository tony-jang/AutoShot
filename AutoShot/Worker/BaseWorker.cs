using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShot.Worker
{
    abstract class BaseWorker
    {
        /// <summary>
        /// 작업을 수행중 작업 목적에 해당하는 결과를 발견했습니다.
        /// </summary>
        public event EventHandler<WorkEventArgs> Find;
        protected virtual void OnFind(WorkEventArgs e)
        {
            Find?.Invoke(this, e);
        }
    }
}
