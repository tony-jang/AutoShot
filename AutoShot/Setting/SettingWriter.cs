using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.ComponentModel;
using Microsoft.Win32;

namespace AutoShot.Setting
{
    class SettingWriter
    {
        public SettingWriter(Setting setting, string FileName = "setting.aucasetting")
        {
            FileInfo fi = new FileInfo(FileName);

            if (FileName == "setting.aucasetting")
            {
                if (fi.DirectoryName == Environment.SystemDirectory)
                {
                    string reg = Registry.GetValue(Registry.CurrentUser.ToString() + "\\AutoCapturer", "SettingLocation", "NotFound").ToString();

                    fi = new FileInfo(reg);
                }
                RegistryKey key;
                key = Registry.CurrentUser.CreateSubKey("AutoCapturer");
                key.SetValue("SettingLocation", fi.FullName);
            }

            setting.RemoveAllEvents();
            var bf = new BinaryFormatter();

            var ms = new MemoryStream();
            bf.Serialize(ms, setting);
            try
            {
                FileStream fs = new FileStream(FileName, FileMode.Create);

                ms.WriteTo(fs);

                fs.Close();
            }
            catch (Exception)
            {
                Globals.Globals.MsgBox("파일 엑세스 충돌이 발생했습니다.\n계속해서 문제가 발생시 오류 상황을 uutak2000@naver.com로 보내주세요.", "엑세스 오류");

                Globals.Globals.MsgBox("제안 : 실행 - regedit으로 HKEY_CURRENT_USER\\AutoCapturer\\SettingLocation의 값을 바꿔보세요.\n그래도 해결이 되지 않을시 이메일로 문제 상황을 보내주시기 바랍니다.", "엑세스 오류");
            }
        }
        

    }
}
