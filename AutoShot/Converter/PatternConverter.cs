using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoShot.Globals.Globals;

namespace AutoShot.Converter
{
    public static class PatternConverter
    {
        public static string Convert(string Variable)
        {
            string Output;
            DateTime dt = DateTime.Now;
            switch (Variable)
            {
                case "%d":
                    Output = string.Format("{0:MMdd}", dt);
                    break;
                case "%D":
                    Output = string.Format("{0:yyyyMMdd}", dt);
                    break;
                case "%t":
                    if (dt.Hour >= 12) { Output = string.Format("{0:오후 hh시 mm분}", dt); }
                    else { Output = string.Format("{0:오전 hh시 mm분}", dt); }
                    break;
                case "%T":
                    Output = string.Format("{0:HH시 mm분}", dt);
                    break;
                case "%a":
                    Output = string.Format("{0:yyyyMMdd HH시 mm분 ss초}", dt);
                    break;
                case "%s":
                    Output = string.Format("{0:HH시 mm분 ss초}", dt);
                    break;
                case "%%":
                    Output = "%";
                    break;
                default:
                    Output = string.Empty;
                    break;
            }

            return Output;
        }
        public static bool Convert(string Variable, out string Output)
        {
            Output = Convert(Variable);

            if (string.IsNullOrEmpty(Output)) { return false; } else { return true; }
        }



        public static PatternError ConvertAll(string MultiVariable, out string Output)
        {
            PatternError Err = PatternError.NoError;

            Output = "";

            if (string.IsNullOrEmpty(MultiVariable)) { Err = PatternError.BlankString; return Err; }


            bool AutoUsed = false;

            for (int i = 0; i <= MultiVariable.Length - 1; i++)
            {
                string VribleCheckStr, CheckChar = MultiVariable.Substring(i, 1);

                if (i + 2 > MultiVariable.Length) VribleCheckStr = MultiVariable.Substring(i);
                else VribleCheckStr = MultiVariable.Substring(i, 2);

                string[] checkStr = { "%d", "%D", "%s", "%t", "%T", "%a", "%%" };
                bool flag = false;

                foreach (string str in checkStr)
                    if (VribleCheckStr == str) {
                        if (AutoUsed == true) { Err = PatternError.AlreadyAutoUsed; return Err; }
                        if (VribleCheckStr == "%a") AutoUsed = true; 
                        flag = true;
                        break;
                    }

                foreach (string CheckAccstr in NotAccessStr.ToCharArray().Select((elem) => elem.ToString()))
                    if (CheckChar == CheckAccstr) { Err = PatternError.CannotAccessString; return Err; }

                if (flag)
                {
                    Output += Convert(VribleCheckStr);

                    i++;
                }
                else if (!flag)
                {

                    if (VribleCheckStr.Substring(0, 1) == "%")
                    {
                        Err = PatternError.UnknownVariable; return Err;
                    }
                    Output += MultiVariable.Substring(i, 1);
                    continue;
                }
            }
            
            return Err;
        }
        


    }
}
namespace AutoShot
{
    public enum PatternError
    {
        /// <summary>
        /// 오류가 포함되어 있지 않습니다.
        /// </summary>
        NoError = 0,
        /// <summary>
        /// 이미 %a 변수가 사용되었으므로 더이상 사용하지 못합니다.
        /// </summary>
        AlreadyAutoUsed = 1,
        /// <summary>
        /// 파일 이름에 사용하지 못하는 문자가 포함되어 있습니다.
        /// </summary>
        CannotAccessString = 2,
        /// <summary>
        /// 파일 이름이 빈칸을 사용하고 있습니다.
        /// </summary>
        BlankString = 3,
        /// <summary>
        /// 알 수 없는 변수를 사용하였습니다.
        /// </summary>
        UnknownVariable = 4

    }
}