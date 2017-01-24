using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace AutoCapturer.Setting
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
            if (string.IsNullOrEmpty(FileName) || !File.Exists(FileName)) return null;
            var bf = new BinaryFormatter();
            var fs = new FileStream(FileName, FileMode.Open);
            try
            {
                Setting sting = bf.Deserialize(fs) as Setting;
                fs.Close();
                return sting;
            }
            catch
            {
                return null;
            }
            

            
        }
    }
}
