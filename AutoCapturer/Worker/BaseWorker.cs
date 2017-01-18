using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCapturer.Worker
{
    abstract class BaseWorker
    {
        /// <summary>
        /// 일을 시작합니다.
        /// </summary>
        public abstract void Work();

        /// <summary>
        /// 일을 잠시 휴식합니다.
        /// </summary>
        public abstract void Rest();

        /// <summary>
        /// 일을 중단합니다.
        /// </summary>
        public abstract void Stop();
        /// <summary>
        /// 일을 하다가 특별한 것을 발견했습니다 (Worker가 해야 할 일을 발견했습니다.)
        /// </summary>
        public event EventHandler<WorkEventArgs> Find;
        protected virtual void OnFind(WorkEventArgs e)
        {
            EventHandler<WorkEventArgs> handler = Find;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
