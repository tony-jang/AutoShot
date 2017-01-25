using AutoCapturer.Windows;
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
using static AutoCapturer.Globals.Globals;

namespace AutoCapturer.Worker
{
    class ImgFromURLWorker : BaseWorker
    {
        Thread thr;
        public string LastHandledURL;
        public ImgFromURLWorker()
        {
            thr = new Thread(() =>
            {

                do
                {
                    string txt;
                    try
                    {
                        txt = Clipboard.GetText();
                    }
                    catch (COMException)
                    {
                        txt = "";
                    }
                    catch (SEHException)
                    {
                        txt = "";
                    }
                    catch (Exception ex)
                    {
                        txt = "";
                        MsgBox(ex.ToString());
                    }

                    if ((txt == LastHandledURL || string.IsNullOrEmpty(txt)) && (!(Keyboard.IsKeyDown(Key.LeftCtrl) && Keyboard.IsKeyDown(Key.C)))) continue;
                    try
                    {
                        if (Globals.ImageUtilities.IsImage(txt))
                        {
                            Uri url = new Uri(txt, UriKind.Absolute); //new Uri("http://google.com", UriKind.Absolute);
                            var image = new BitmapImage(url);

                            System.Drawing.Size size = Globals.ImageUtilities.GetWebDimensions(url);

                            LastHandledURL = txt;
                            OnFind(new ImageSizeEventArgs(new Size(size.Width, size.Height), image));
                        }
                    }
                    catch (Exception ex) {
                        MessageBox.Show(ex.ToString()); }
                    Thread.Sleep(10);
                } while (true);
            });
        }

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
