using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoCapturer.Converter
{
    public static class PatternConverter
    {
        /// <summary>
        /// 사용 할 수 없는 String 모음
        /// </summary>
        private static string NotAccessStr = @"\/:*?""<>|";
        public static bool Convert(string Variable, out string Output)
        {
            //string dateTime = "01/08/2016 1:50:50.42"; DateTime dte = System.Convert.ToDateTime(dateTime);
            DateTime dt = DateTime.Now;
            switch (Variable)
            {
                case "%d":
                    Output = string.Format("{0:MMdd}",dt);
                    break;
                case "%D":
                    Output = string.Format("{0:yyyyMMdd}", dt);
                    break;
                case "%t":
                    if (dt.Hour >= 12){ Output = string.Format("{0:hh:mm}PM", dt); }
                    else { Output = string.Format("{0:hh:mm}AM",dt); }
                    break;
                case "%T":
                    Output = string.Format("{0:HH:mm}", dt);
                    break;
                case "%a":
                    Output = string.Format("{0:yyyyMMdd HH:mm:ss}",dt);
                    break;
                case "%%":
                    Output = "%";
                    break;
                default:
                    Output = string.Empty;
                    return false;
            }
            
            return true;
        }
        

        public static bool ConvertAll(string MultiVariable, out string Output)
        {
            Output = "";

            bool AutoUsed = false;

            for (int i = 0; i <= MultiVariable.Length-1; i++)
            {

                string s;
                if (i + 2 > MultiVariable.Length) s = MultiVariable.Substring(i);
                else s = MultiVariable.Substring(i, 2);

                string[] checkStr = {"%d","%D", "%t","%T","%a","%%"};
                bool flag = false;
                foreach (string str in checkStr)
                {
                    if (s == "%a") { AutoUsed = true; }
                    else if (s == str) {
                        if (AutoUsed == true) return false;
                        flag = true;
                            break;
                        
                    }
                    
                }
                if (flag)
                {
                    string AddedStr;
                    Convert(s, out AddedStr);

                    Output += AddedStr;
                    i++;
                }
                else if (!flag)
                {
                    Output += MultiVariable.Substring(i, 1);
                    continue;
                }
            }
            
            return true;
        }

        [Flags]
        public enum ErrorList
        {
            /// <summary>
            /// 이미 %a 변수가 사용되었으므로 더이상 사용하지 못합니다.
            /// </summary>
            AlreadyAutoUsed = 0,
            /// <summary>
            /// 파일 이름에 사용하지 못하는 문자가 포함되어 있습니다.
            /// </summary>
            CannotAccessString = 1,
            /// <summary>
            /// 알 수 없는 변수를 사용하였습니다.
            /// </summary>
            UnknownVariable = 2,
            
        }

    }
}
