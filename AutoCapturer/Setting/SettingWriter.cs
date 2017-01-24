using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace AutoCapturer.Setting
{
    class SettingWriter
    {
        public SettingWriter(Setting setting, string FileName = "setting.aucasetting")
        {
            setting.RemoveAllEvents();
            var bf = new BinaryFormatter();

            var ms = new MemoryStream();
            bf.Serialize(ms, setting);

            FileStream fs = new FileStream(FileName, FileMode.Create);

            ms.WriteTo(fs);

            fs.Close();

        }
        

    }
}
