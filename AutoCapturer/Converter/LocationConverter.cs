using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutoCapturer.Globals.Globals;


namespace AutoCapturer.Converter
{
    public static class LocationConverter
    {
        public static string Convert(string Variable)
        {
            string Output;
            DateTime dt = DateTime.Now;
            switch (Variable)
            {
                case "%d":
                    Output = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    break;
                case "%m":
                    Output = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    break;
                case "":
                case "%a":
                    Output = AppDomain.CurrentDomain.BaseDirectory;
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

        public static ErrorList ConvertAll(string MultiVariable, out string Output)
        {
            ErrorList Err = ErrorList.NoError;

            Output = "";

            if (string.IsNullOrEmpty(MultiVariable)) { Output = Convert("%a"); return Err; }


            for (int i = 0; i <= MultiVariable.Length - 1; i++)
            {
                string VribleCheckStr, CheckChar = MultiVariable.Substring(i, 1);

                if (i + 2 > MultiVariable.Length) VribleCheckStr = MultiVariable.Substring(i);
                else VribleCheckStr = MultiVariable.Substring(i, 2);

                string[] checkStr = { "%d", "%m", "%a", "%%" };
                bool flag = false;

                foreach (string str in checkStr)
                    if (VribleCheckStr == str)
                    {
                        flag = true;
                        break;
                    }

                foreach (string CheckAccstr in NotAccessStr.ToCharArray().Select((elem) => elem.ToString()))
                    if (CheckChar == CheckAccstr && CheckAccstr != "\\") { Err = ErrorList.CannotAccessString; return Err; }

                if (flag)
                {
                    Output += Convert(VribleCheckStr);

                    i++;
                }
                else if (!flag)
                {

                    if (VribleCheckStr.Substring(0, 1) == "%")
                    {
                        Err = ErrorList.UnknownVariable; return Err;
                    }
                    Output += MultiVariable.Substring(i, 1);
                    continue;
                }
            }

            return Err;
        }
        
    }

}
