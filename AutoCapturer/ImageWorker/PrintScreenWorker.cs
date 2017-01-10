using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using f=System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using static AutoCapturer.Interop.UnsafeNativeMethods;

namespace AutoCapturer.Observer
{
    class PrintScreenWorker
    {


        public event AEventHandler DetectPrtScr;
        public delegate void AEventHandler(ImageSource DetectedImage);

        public void StartObserving()
        {
        
            Thread thr = new Thread(new ThreadStart(() =>
            {
                do
                {
                    if (DetectPrtScr != null && GetAsyncKeyState((int)f.Keys.PrintScreen) == -32767)
                    {
                        do
                        {
                            if (Clipboard.ContainsImage())
                            {
                                Thread.Sleep(200);
                                BitmapSource bmp = null;

                                do { } while (!GetClipboardImage(ref bmp));

                                f.IDataObject clipboardData = f.Clipboard.GetDataObject();
                                if (clipboardData != null)
                                {
                                    if (clipboardData.GetDataPresent(f.DataFormats.Bitmap))
                                    {
                                        Bitmap bitmap = (Bitmap)clipboardData.GetData(f.DataFormats.Bitmap);
                                        
                                        try
                                        {
                                            DetectPrtScr(System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions()));
                                        }
                                        catch (NullReferenceException)
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
                img = Clipboard.GetImage();   
            }
            catch
            {
                return false;
            }
            
            return true;
        }
    }
}
