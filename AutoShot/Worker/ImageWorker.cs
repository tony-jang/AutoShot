using AutoShot.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Input;
using static AutoShot.Globals.Globals;
using AutoShot.Utils;

namespace AutoShot.Worker
{
    class ImageWorker : BaseWorker
    {
        public ImageWorker()
        {
            ClipboardMonitor.OnClipboardChange += ClipboardMonitor_OnClipboardChange;
        }

        private void ClipboardMonitor_OnClipboardChange(ClipboardFormat format, object data)
        {
            if (format == ClipboardFormat.Text)
            {
                string s = (string)data;

                if (Globals.ImageUtilities.IsImage(s))
                {
                    Uri url = new Uri(s, UriKind.Absolute);
                    var image = new BitmapImage(url);

                    System.Drawing.Size size = Globals.ImageUtilities.GetWebDimensions(url);

                    OnFind(new ImageSizeEventArgs(new Size(size.Width, size.Height), image));
                }
            }
        }
    }

    public class ImageSizeEventArgs : WorkEventArgs
    {
        public ImageSizeEventArgs(Size imagesize, BitmapImage img)
        {
            ImageSize = imagesize;
            Img = img;
        }
        public Size ImageSize;
        public BitmapImage Img;
    }
}
