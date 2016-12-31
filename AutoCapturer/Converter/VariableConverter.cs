using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCapturer.Converter
{
    public static class VariableConverter
    {
        private static string NotAccessStr = @"\/:*?""<>|";
        public static bool Convert(string Variable, out string OutPut)
        {
            //string dateTime = "01/08/2016 1:50:50.42"; DateTime dte = System.Convert.ToDateTime(dateTime);
            DateTime dt = DateTime.Now;
            switch (Variable)
            {
                case "%d":
                    OutPut = string.Format("{0:MMdd}",dt);
                    break;
                case "%D":
                    OutPut = string.Format("{0:yyyyMMdd}", dt);
                    break;
                case "%t":
                    if (dt.Hour >= 12)
                    {
                        OutPut = string.Format("{0:hh:mm}PM", dt);
                    }
                    else
                    {
                        OutPut = string.Format("{0:hh:mm}AM",dt);
                    }
                    break;

                default:
                    OutPut = string.Empty;
                    break;
            }
            
            return true;
        }
    }
}
