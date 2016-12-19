using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AutoCapturer.Effectors
{
    class RotateEffector : BaseEffector
    {

        public override BitmapImage ApplyEffect()
        {
            var biOriginal = Source;
            var biRotated = new BitmapImage();
            biRotated.BeginInit();
            biRotated.UriSource = biOriginal.UriSource;
            biRotated.Rotation = Rotation.Rotate180;
            biRotated.EndInit();
            return biRotated;
        }
    }    
}
