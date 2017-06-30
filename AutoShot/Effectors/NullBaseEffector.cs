using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AutoShot.Effectors
{
    public class NullBaseEffector : BaseEffector
    {
        public override BitmapImage ApplyEffect()
        {
            return this.Source;
        }
    }
}
