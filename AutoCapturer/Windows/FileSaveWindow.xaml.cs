using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static AutoCapturer.Globals.Globals;

namespace AutoCapturer.Windows
{
    /// <summary>
    /// FileSaveWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class FileSaveWindow : ChromeWindow
    {
        public FileSaveWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            locTB.Text = ShowSelectDirectoryDialog()?.FullName;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            flag = false;
            this.Close();
        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            string location = locTB.Text;

            if (string.IsNullOrEmpty(location)) location = Environment.CurrentDirectory;

            CurrentSetting = new Setting.Setting(new Setting.SavePattern("A", "%d"));
            if (!System.IO.Directory.Exists(location)) { MsgBox("폴더가 존재하지 않습니다!", "폴더 없음!"); return; }
            try
            {
                var sw = new Setting.SettingWriter(CurrentSetting, System.IO.Path.Combine(locTB.Text, FileNameTB.Text + ".aucasetting"));
            }
            catch (Exception)
            {
                MsgBox("알 수 없는 오류가 발생했습니다!", "오류 발생!");
                return;
            }
            

            flag = true;
            this.Close();
        }
        bool flag = false;
        public new bool ShowDialog()
        {
            base.ShowDialog();
            return flag;
        }
    }
}
