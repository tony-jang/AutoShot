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
    /// InitSettingWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InitSettingWindow : ChromeWindow
    {

        Setting.Setting ReturnSetting = new Setting.Setting(new Setting.SavePattern("캡쳐(%t)", "%d"));

        public InitSettingWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void NextBtn_Click(object sender, RoutedEventArgs e)
        {
            Grids.SelectedIndex = int.Parse(((Button)sender).Tag.ToString()) + 1;
        }

        private void PrevBtn_Click(object sender, RoutedEventArgs e)
        {
            Grids.SelectedIndex = int.Parse(((Button)sender).Tag.ToString()) - 1;
        }

        private void BtnAllSkip_Click(object sender, RoutedEventArgs e)
        {
            if (MsgBox("정말 모두 건너뛰시겠습니까?\n만약 지금까지 설정한 내용이 있다면 설정된 내용까지만 반영됩니다.", "모두 건너뛰기", Globals.MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
            {
                this.Close();
            }
        }

        public new Setting.Setting ShowDialog()
        {
            base.ShowDialog();
            return ReturnSetting;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveLocTB.Text = ShowSelectDirectoryDialog()?.FullName;
        }
    }
}
