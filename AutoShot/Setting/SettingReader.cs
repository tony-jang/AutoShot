using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows;
using static AutoShot.Globals.Globals;
using Microsoft.Win32;

namespace AutoShot.Setting
{
    class SettingReader
    {
        public SettingReader(string FileName = "setting.aucasetting")
        {
            this.FileName = FileName;
        }
        private string FileName;
        public Setting ReadSetting()
        {
            if (FileName == "setting.aucasetting")
            {
                //RegistryKey key;
                //key ;
                //key.SetValue("SettingLocation", fi.FullName);

                object reg = Registry.GetValue(Registry.CurrentUser.ToString() + "\\AutoCapturer","SettingLocation","NotFound");

                if (reg == null)
                    return null;
                else if (reg.ToString() == "NotFound")
                    return null;

                FileName = reg.ToString();
            }

            if (string.IsNullOrEmpty(FileName) || !File.Exists(FileName))
            {
                return null;
            }
            var bf = new BinaryFormatter();
            var fs = new FileStream(FileName, FileMode.Open);
            try
            {
                Setting sting = bf.Deserialize(fs) as Setting;
                fs.Close();
                return sting;
            }
            catch (Exception ex)
            {
                fs.Close();
                return null;
            }
            

            
        }
    }
}
