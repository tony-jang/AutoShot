using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AutoCapturer.Windows
{
    /// <summary>
    /// ImageEditor.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageEditor : Windows.ChromeWindow
    {
        public ImageEditor()
        {
            InitializeComponent();
            Editor.ImageEditMode = UserControls.EditMode.SizeChange;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Editor.ImageStretchMode = (Stretch)comboBox.SelectedIndex;
        }

        private void comboBox_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Editor.ImageEditMode = (UserControls.EditMode)comboBox_Copy.SelectedIndex;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "png 파일|*.png|jpg 파일|*.jpg";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Editor.image = new BitmapImage(new Uri(ofd.FileName));
                EditTB.Text = "현재 편집 중인 파일 : " + ofd.FileName;
            }
        }
    }
}
