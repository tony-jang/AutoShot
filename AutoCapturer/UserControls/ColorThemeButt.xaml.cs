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

namespace AutoCapturer.UserControls
{
    /// <summary>
    /// ColorThemeButt.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ColorThemeButt : UserControl
    {
        public ColorThemeButt()
        {
            InitializeComponent();
        }

        private bool _Checked = true;
        public bool ThemeChecked
        {
            get { return _Checked; }
            set {_Checked = value; }
        }
        
        public string ThemeName
        {
            get { return tbThemeName.Text; }
            set { tbThemeName.Text = value; }
        }
        
        public Brush WdwSurface
        {
            get { return Rect_Surf.Background; }
            set { Rect_Surf.Background = value; }
        }

        public Brush WdwInner
        {
            get { return Rect_Inner.Fill; }
            set { Rect_Inner.Fill = value; }
        }


        private void Rect_Click_MouseDown(object sender, MouseButtonEventArgs e)
        {

        }
    }
}
