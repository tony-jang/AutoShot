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

        public List<ShortCutKey> keys;

        public ShortCutWorker() : this(new List<ShortCutKey>()) { }
        public ShortCutWorker(ShortCutKey key) : this(new List<ShortCutKey>(new ShortCutKey[] { key })) { }
        public ShortCutWorker(ShortCutKey[] keys) : this(keys.ToList()) { }
        public ShortCutWorker(List<ShortCutKey> keys)
        {
            this.keys = keys;

            thr = new Thread(() =>
            {
                do
                {
                    foreach(ShortCutKey key in keys)
                    {
                        if (key.SecondKey == Key.None)
                        {
                            if (Keyboard.IsKeyDown(key.FirstKey))
                            {
                                OnFind(new ShortCutWorkEventArgs(key));
                            }
                        }
                        else
                        {
                            if (Keyboard.IsKeyDown(key.FirstKey) && Keyboard.IsKeyDown(key.FirstKey))
                            {
                                OnFind(new ShortCutWorkEventArgs(key));
                            }
                        }
                    }
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
            thr.Start();
        }
    }

    class ShortCutWorkEventArgs : WorkEventArgs
    {
        public ShortCutWorkEventArgs(ShortCutKey data)
        {
            Data = data;
        }
        ShortCutKey Data;
    }


    class ShortCutKey
    {
        private Key _FirstKey, _SecondKey;

        private static List<string> SeparateKeys = new List<string>();

        public Key FirstKey
        {
            get { return _FirstKey; }
            set { _FirstKey = value; }
        }

        public Key SecondKey
        {
            get { return _SecondKey; }
            set { _SecondKey = value; }
        }

        private string _SeparateKey;
        /// <summary>
        /// 해당 쇼트컷을 중복방지용 Key로 사용됩니다.
        /// </summary>
        public string SeparateKey
        {
            get { return _SeparateKey; }
            set { _SeparateKey = value; }
        }


        public ShortCutKey(Key key1, Key key2,string SeparateKey)
        {
            FirstKey = key1;
            SecondKey = key2;

            if (SeparateKeys.Contains(SeparateKey)) throw new Exception("이미 선언된 키가 중복되어 사용되었습니다.");

            SeparateKeys.Add(SeparateKey);
        }

        public ShortCutKey(Key key1, string SeparateKey) : this(key1, Key.None, SeparateKey) { }
    }

}
