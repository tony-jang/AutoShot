using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AutoCapturer.Collections
{
    [Serializable]
    public class NotifyList<T> : List<T> , ICloneable
    {
        public NotifyList()
        { }
        public NotifyList(IEnumerable<T> collection) : base(collection)
        { }

        public new void Insert(int index, T item)
        {
            base.Insert(index, item);
            OnListChanged(item, ChangeAction.Add);
        }

        public new void Add(T item)
        {
            base.Add(item);
            OnListChanged(item, ChangeAction.Add);
        }
        public new void Remove(T item)
        {
            base.Remove(item);
            OnListChanged(item, ChangeAction.Add);
        }

        List<EventHandler<ChangeEventArgs<T>>> delegates = new List<EventHandler<ChangeEventArgs<T>>>();

        public void RemoveAllEvents()
        {
            foreach (EventHandler<ChangeEventArgs<T>> events in delegates)
            {
                _ListChanged -= events;
            }
            MessageBox.Show(this.Count.ToString());
            delegates.Clear();
        }

        private event EventHandler<ChangeEventArgs<T>> _ListChanged;
        public event EventHandler<ChangeEventArgs<T>> ListChanged
        {
            add
            {
                _ListChanged += value;
                delegates.Add(value);
            }
            remove
            {
                _ListChanged -= value;
                delegates.Remove(value);
            }
        }

        protected void OnListChanged(T item, ChangeAction action)
        {
            if (_ListChanged != null) _ListChanged(this, new ChangeEventArgs<T>(item, action));
        }

        public object Clone()
        {
            NotifyList<T> list = new NotifyList<T>();

            foreach(T itm in this)
            {
                list.Add(itm);
            }

            return list;
        }
    }
}
