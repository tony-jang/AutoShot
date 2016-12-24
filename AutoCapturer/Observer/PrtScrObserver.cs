using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace AutoCapturer.Observer
{
    class PrtScrObserver
    {
        [DllImport("user32")]
        public static extern short GetAsyncKeyState(Int32 vKey);

        public event AEventHandler DetectPrtScr;
        public delegate void AEventHandler(ImageSource DetectedImage);

        public void TestMtd()
        {
            Thread thr = new Thread(() =>
            {
                do
                {
                    if (DetectPrtScr != null && GetAsyncKeyState((int)Keys.PrintScreen) == -32767)
                    {
                        do {
                            if (System.Windows.Clipboard.ContainsImage())
                            {
                                // ImageUIElement.Source = Clipboard.GetImage(); // does not work


                                BitmapSource bmp = null;

                                do { } while (!GetClipboardImage(ref bmp));

                                System.Windows.Forms.IDataObject clipboardData = System.Windows.Forms.Clipboard.GetDataObject();
                                if (clipboardData != null)
                                {
                                    if (clipboardData.GetDataPresent(System.Windows.Forms.DataFormats.Bitmap))
                                    {
                                        System.Drawing.Bitmap bitmap = (System.Drawing.Bitmap)clipboardData.GetData(System.Windows.Forms.DataFormats.Bitmap);
                                        DetectPrtScr( System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
                                        
                                        break;
                                    }
                                }
                            }
                            Thread.Sleep(1);
                        } while (true);
                        
                    }
                    Thread.Sleep(1);
                } while (true);
                
            });

            thr.SetApartmentState(ApartmentState.STA);
            thr.Start();

        }

        public bool GetClipboardImage(ref BitmapSource img)
        {
            
            try
            {
                img = System.Windows.Clipboard.GetImage();   
            }
            catch
            {
                return false;
            }
            
            return true;
        }
    }
}
