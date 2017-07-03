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
        public event EventHandler KeyChanged;

        public void OnKeyChanged()
        {
            KeyChanged?.Invoke(this, new EventArgs());
        }

        public bool Control
        {
            get => cbKey1.IsChecked.Value;
            set => cbKey1.IsChecked = value;
        }

        public bool Alt
        {
            get => cbKey2.IsChecked.Value;
            set => cbKey2.IsChecked = value;
        }

        public bool Shift
        {
            get => cbKey3.IsChecked.Value;
            set => cbKey3.IsChecked = value;
        }

        public Key Key
        {
            get => (Key)btnKey.Tag;
            set
            {
                btnKey.Tag = value;
                btnKey.Content = value.ToString() + " Key";
            }
        }

        public ShortcutKey()
        {
            InitializeComponent();
            cbKey1.Checked += CbKey_Checked;
            cbKey1.Unchecked += CbKey_Checked;
            cbKey2.Checked += CbKey_Checked;
            cbKey2.Unchecked += CbKey_Checked;
            cbKey3.Checked += CbKey_Checked;
            cbKey3.Unchecked += CbKey_Checked;

        }

        private void CbKey_Checked(object sender, RoutedEventArgs e)
        {
            if (!InitStart)
                OnKeyChanged();
        }

        private void btnKey_Click(object sender, RoutedEventArgs e)
        {
            btnKey.Tag = ShowKeyInput((Key)btnKey.Tag);
            btnKey.Content = ((Key)btnKey.Tag).ToString() + " Key";
            OnKeyChanged();
        }
        bool InitStart = false;
        internal void InitalizeData(Key WPFKey, bool control, bool alt, bool shift)
        {
            InitStart = true;
            btnKey.Tag = WPFKey;
            btnKey.Content = WPFKey.ToString() + " Key";
            cbKey1.IsChecked = control;
            cbKey2.IsChecked = alt;
            cbKey3.IsChecked = shift;
            InitStart = false;
        }
    }
}
