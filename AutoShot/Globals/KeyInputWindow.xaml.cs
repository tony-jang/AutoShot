using AutoShot.Windows;
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

namespace AutoShot.Globals
{
    /// <summary>
    /// KeyInputWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class KeyInputWindow : ChromeWindow
    {
        public KeyInputWindow(Key key = Key.None)
        {
            InitializeComponent();
            ReturnData = key;
            KeyTB.Text = key.ToString() + " Key";

            FirstKey = key;

            this.PreviewKeyDown += PrevKeyDown;
        }
        Key FirstKey = Key.None;
        private void PrevKeyDown(object sender, KeyEventArgs e)
        {
            if (InputWord(e.Key))
            {
                KeyTB.Text = e.Key.ToString() + " Key";
                ReturnData = e.Key;
                e.Handled = true;
            }

            bool InputWord(Key key)
            {
                int k = (int)key;

                if ((k >= (int)Key.A && k <= (int)Key.Z) ||
                    (k >= (int)Key.D0 && k <= (int)Key.D9) ||
                    (k >= (int)Key.F1 && k <= (int)Key.F24) ||
                    (k >= (int)Key.NumPad0 && k <= (int)Key.NumPad9) ||
                    (k == (int)Key.Tab))
                {
                    return true;
                }
                return false;
            }
        }

        public void BtnClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content.ToString())
            {
                case "확인":

                    break;
                case "취소":
                    ReturnData = FirstKey;
                    break;
            }
            this.Close();
        }
        public Key ReturnData;
        public new Key ShowDialog()
        {
            base.ShowDialog();
            return ReturnData;
        }
    }

    static partial class Globals
    {
        public static Key ShowKeyInput(Key key = Key.None)
        {
            var kw = new KeyInputWindow(key);
            return kw.ShowDialog();
        }
    }
}
