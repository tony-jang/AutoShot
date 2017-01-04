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

        public void StartObserving()
        {
        
            Thread thr = new Thread(new ThreadStart(() =>
            {
                do
                {
                    if (DetectPrtScr != null && GetAsyncKeyState((int)Keys.PrintScreen) == -32767)
                    {
                        do
                        {
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


                                        try
                                        {
                                            DetectPrtScr(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
                                        }
                                        catch (NullReferenceException e)
                                        {
                                            
                                            Globals.Globals.MainDispatcher.Invoke(new Action(() =>
                                            {
                                                PopUps.PopUpWdw wdw = new PopUps.PopUpWdw("알 수 없는 오류", "알 수 없는 오류가 발생했습니다.");
                                                wdw.ShowTime(1000);
                                            }));
                                            
                                            
                                        }


                                        break;
                                    }
                                }
                            }
                            Thread.Sleep(1);
                        } while (true);

                    }
                    Thread.Sleep(1);
                } while (true);

            }));

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
