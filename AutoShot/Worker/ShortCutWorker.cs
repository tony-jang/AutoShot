using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

using f = System.Windows.Forms;

using Gma.System.MouseKeyHook;

namespace AutoShot.Worker
{
    class ShortCutWorker : BaseWorker
    {
        public List<Shotcut> Keys { get; set; }
        private List<Shotcut> _DownKeys = new List<Shotcut>();
        static IKeyboardMouseEvents hook;

        public bool IsUsed = true;

        public ShortCutWorker() : this(new List<Shotcut>()) { }
        public ShortCutWorker(Shotcut key) : this(new List<Shotcut>(new Shotcut[] { key })) { }
        public ShortCutWorker(Shotcut[] keys) : this(keys.ToList()) { }
        public ShortCutWorker(List<Shotcut> keys)
        {
            this.Keys = keys;

            hook = Hook.GlobalEvents();
            hook.KeyDown += ShortCutWorker_KeyDown;

            //thr = new Thread(() =>
            //{
            //    do
            //    {
            //        if (IsUsed)
            //        {
            //            foreach (ShortCutKey key in keys)
            //            {
            //                if (key.IsDisabled || IsAllNone(key.FirstKey, key.SecondKey)) continue;
            //                if (_DownKeys.Contains(key))
            //                {
            //                    if (IsKeyUp(key.FirstKey) || IsKeyUp(key.SecondKey))
            //                        _DownKeys.Remove(key);

            //                    continue;
            //                }
            //                if (IsKeyDown(key.FirstKey) && IsKeyDown(key.SecondKey))
            //                {
            //                    _DownKeys.Add(key);
            //                    OnFind(new ShortCutWorkEventArgs(key));
            //                }
            //            }
            //        }
            //        Thread.Sleep(10);
            //    } while (true);
            //});
        }

        private void ShortCutWorker_KeyDown(object sender, f.KeyEventArgs e)
        {
            foreach (Shotcut s in Keys)
            {
                if (s.WPFKey == Globals.Globals.WPFKeyFromWFormKey(e.KeyCode) &&
                    s.Control == e.Control &&
                    s.Alt == e.Alt &&
                    s.Shift == e.Shift)
                {
                    OnFind(new ShortCutWorkEventArgs(s));
                    break;
                }
            }
            
        }
    }

    class ShortCutWorkEventArgs : WorkEventArgs
    {
        public ShortCutWorkEventArgs(Shotcut data)
        {
            _Data = data;
        }
        Shotcut _Data;

        public Shotcut Data
        {
            get { return _Data; }
            set { _Data = value; }
        }
    }

    [Serializable]
    public class Shotcut : ICloneable
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
                _ValueChangedEvent -= beh;

            delegates.Clear();
        }
        public void ValueChanged()
        {
            _ValueChangedEvent?.Invoke();
        }

        public object Clone()
        {
            return new Shotcut(this.Name, this.WPFKey, this.Control, this.Alt, this.Shift);
        }

        private bool _IsDisabled = false;
        public bool IsDisabled
        {
            get { return _IsDisabled; }
            set { _IsDisabled = value; }
        }
        Key _WPFKey;
        public string Name { get; set; }
        public bool Control { get; set; }
        public bool Alt { get; set; }
        public bool Shift { get; set; }
        public Key WPFKey
        {
            get { return _WPFKey; }
            set { _WPFKey = value; ValueChanged(); }
        }
        public Shotcut(string name, Key key1, bool control, bool alt, bool shift)
        {
            this.Name = name;
            _WPFKey = key1;
            this.Control = control;
            this.Alt = alt;
            this.Shift = shift;
        }
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
