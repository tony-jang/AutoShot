using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCapturer.ImageWorker
{
    abstract class BaseImageWorker
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
    }
}
