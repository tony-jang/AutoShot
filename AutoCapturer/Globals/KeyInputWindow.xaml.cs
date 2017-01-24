using AutoCapturer.Windows;
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

namespace AutoCapturer.Globals
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
            this.PreviewKeyDown += PrevKeyDown;
            
        }
        private void PrevKeyDown(object sender, KeyEventArgs e)
        {
            Key k = e.Key;
            if (k == Key.Escape)
            {
                ReturnData = Key.None;
                this.Close();
                return;
            }
            if (k == Key.System) k = Key.LeftAlt;
            KeyTB.Text = k.ToString() + " Key";
            ReturnData = k;
            e.Handled = true;
        }

        public void BtnClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content.ToString())
            {
                case "확인":

                    break;
                case "취소":
                    ReturnData = Key.None;
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
