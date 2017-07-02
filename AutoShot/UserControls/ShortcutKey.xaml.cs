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
using System.Windows.Navigation;
using System.Windows.Shapes;

using static AutoShot.Globals.Globals;

namespace AutoShot.UserControls
{
    /// <summary>
    /// ShortcutKey.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ShortcutKey : UserControl
    {
        public bool Control => cbKey1.IsChecked.Value;
        public bool Alt => cbKey2.IsChecked.Value;
        public bool Shift => cbKey3.IsChecked.Value;

        public Key Key => (Key)btnKey.Tag;

        public ShortcutKey()
        {
            InitializeComponent();
        }

        private void btnKey_Click(object sender, RoutedEventArgs e)
        {
            btnKey.Tag = ShowKeyInput((Key)btnKey.Tag);
            btnKey.Content = ((Key)btnKey.Tag).ToString() + " Key";
        }
    }
}
