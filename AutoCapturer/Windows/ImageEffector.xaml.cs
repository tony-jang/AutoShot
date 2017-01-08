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

namespace AutoCapturer.Windows
{
    /// <summary>
    /// ImageEffector.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ImageEffector : ChromeWindow
    {
        public ImageEffector()
        {
            InitializeComponent();
            ImageGrid.MouseWheel += Image_Wheel;
            OrignialSize = new Size(Show_Image.Source.Width, Show_Image.Source.Height);
        }

        double _Ratio = 100.0;
        double Ratio
        {
            get { return _Ratio; }
            set { _Ratio = value; if (value < 0) _Ratio = 0; }
        }

        Size OrignialSize;
        private void Image_Wheel(object sender, MouseWheelEventArgs e)
        {
            int MultipleData;
            MultipleData = e.Delta > 0 ? 1 : -1;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {

                Ratio += 20.0 * MultipleData;
                
                TBRatio.Text = $"확대 비율 : {Math.Round(Ratio,2)}%";
                Show_Image.Width = (OrignialSize.Width / 100) * Ratio;
                Show_Image.Height = (OrignialSize.Height / 100) * Ratio;
                Console.WriteLine(MultipleData.ToString());
            }
            else
            {
                Show_Image.Margin = new Thickness(Show_Image.Margin.Left, Show_Image.Margin.Top + 10 * MultipleData, 
                    Show_Image.Margin.Right, Show_Image.Margin.Bottom);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
