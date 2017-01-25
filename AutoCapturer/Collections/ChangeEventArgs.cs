using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCapturer.Collections
{
    public class ChangeEventArgs<T> : EventArgs
    {
        public T Item { get; }
        public ChangeAction Action { get; }

        public ChangeEventArgs(T item, ChangeAction action)
        {
            this.Item = item;
            this.Action = action;
        }
    }
}
