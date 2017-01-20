using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AutoCapturer.Effectors
{
    public abstract class BaseEffector
    {
        BitmapImage BmpImg;
        public BaseEffector(BitmapImage bmpImg)
        {
            BmpImg = bmpImg;
        }
        public BaseEffector()
        {

        }

        abstract public BitmapImage ApplyEffect();
        public BitmapImage Source
        {
            get
            {
                return BmpImg;
            }
            set
            {
                BmpImg = value;
            }
        }

    }
}
