using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace AutoCapturer.Worker
{
    class ShortCutWorker : BaseWorker
    {

        private List<ShortCutKey> _keys;
        public List<ShortCutKey> Keys
        {
            get { return _keys; }
            set { _keys = value; }
        }

        private List<ShortCutKey> _DownKeys = new List<ShortCutKey>();

        public bool IsUsed = true;

        public ShortCutWorker() : this(new List<ShortCutKey>()) { }
        public ShortCutWorker(ShortCutKey key) : this(new List<ShortCutKey>(new ShortCutKey[] { key })) { }
        public ShortCutWorker(ShortCutKey[] keys) : this(keys.ToList()) { }
        public ShortCutWorker(List<ShortCutKey> keys)
        {
            this._keys = keys;

            thr = new Thread(() =>
            {
                do
                {
                    if (IsUsed)
                    {
                        foreach (ShortCutKey key in keys)
                        {
                            if (key.IsDisabled || IsAllNone(key.FirstKey, key.SecondKey)) continue;
                            if (_DownKeys.Contains(key))
                            {
                                if (IsKeyUp(key.FirstKey) || IsKeyUp(key.SecondKey)) _DownKeys.Remove(key);

                                continue;
                            }
                            if (IsKeyDown(key.FirstKey) && IsKeyDown(key.SecondKey))
                            {
                                _DownKeys.Add(key);
                                OnFind(new ShortCutWorkEventArgs(key));
                            }
                        }
                    }
                    Thread.Sleep(10);
                } while (true);

            });
        }

        Thread thr;
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
            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();
        }


        public bool IsKeyDown(Key key)
        {
            if (key == Key.None) return true;
            return Keyboard.IsKeyDown(key);
        }
        public bool IsKeyUp(Key key)
        {
            if (key == Key.None) return true;
            return Keyboard.IsKeyUp(key);
        }
        public bool IsAllNone(Key key1, Key key2)
        {
            if (key1 == Key.None && key2 == Key.None) return true;

            return false;
        }
    }

    class ShortCutWorkEventArgs : WorkEventArgs
    {
        public ShortCutWorkEventArgs(ShortCutKey data)
        {
            _Data = data;
        }
        ShortCutKey _Data;

        public ShortCutKey Data
        {
            get { return _Data; }
            set { _Data = value; }
        }
    }

    [Serializable]
    public class ShortCutKey : ICloneable
    {
        public delegate void BlankEventHandler();
     

        List<BlankEventHandler> delegates = new List<BlankEventHandler>();

        private event BlankEventHandler _ValueChangedEvent;
        public event BlankEventHandler ValueChangedEvent
        {
            add
            {
                _ValueChangedEvent += value;
                delegates.Add(value);
            }
            remove
            {
                _ValueChangedEvent -= value;
                delegates.Remove(value);
            }
        }

        public void RemoveAllEvents()
        {

            foreach (BlankEventHandler beh in delegates)
            {
                _ValueChangedEvent -= beh;
            }

            delegates.Clear();
        }
        public void ValueChanged()
        {
            if (_ValueChangedEvent != null) _ValueChangedEvent();
        }

        public object Clone()
        {
            return new ShortCutKey(this.FirstKey, this.SecondKey, this.SeparateKey);
                
        }

        private bool _IsDisabled = false;
        public bool IsDisabled
        {
            get { return _IsDisabled; }
            set { _IsDisabled = value; }
        }


        private Key _FirstKey, _SecondKey;

        private static List<string> SeparateKeys = new List<string>();

        public Key FirstKey
        {
            get { return _FirstKey; }
            set { _FirstKey = value; ValueChanged(); }
        }

        public Key SecondKey
        {
            get { return _SecondKey; }
            set { _SecondKey = value; ValueChanged(); }
        }

        private string _SeparateKey;
        /// <summary>
        /// 해당 쇼트컷을 중복방지용 Key로 사용됩니다.
        /// </summary>
        public string SeparateKey
        {
            get { return _SeparateKey; }
        }


        public ShortCutKey(Key key1, Key key2, string SeparateKey)
        {
            FirstKey = key1;
            SecondKey = key2;
            _SeparateKey = SeparateKey;
            SeparateKeys.Add(SeparateKey);
        }

        public ShortCutKey(Key key1, string SeparateKey) : this(key1, Key.None, SeparateKey) { }
    }

    /// <summary>
    /// 선언된 키가 중복되어 사용될때 발생하는 예외를 담은 클래스입니다.
    /// </summary>
    public class DuplicatedKeyException : Exception
    {
        public DuplicatedKeyException(string message) : base(message)
        {

        }
    }
}
