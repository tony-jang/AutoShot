using AutoCapturer.Setting;
using AutoCapturer.UserControls;
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
        Timer tmr = new Timer();
        public ImageEditor()
        {
            InitializeComponent();
            Editor.ImageEditMode = UserControls.EditMode.SizeChange;
            Editor.ImageStretchMode = Stretch.Fill;

            tmr.Interval = 20;
            tmr.Tick += ForDebugChangeSize;
            tmr.Start();

            EditBtn1.Click += EditBtnClick;
            EditBtn2.Click += EditBtnClick;
            EditBtn3.Click += EditBtnClick;
            
            RatioCB.SelectionChanged += SelectionChanging;

            Editor.RatioChanged += Editor_RatioChanged;
            Editor.ImageSizeChanged += Editor_ImageSizeChanged;

            double[] Ratios = { 12.5, 25.0, 33.33, 50.0, 66.67, 100.0, 150.0, 200.0, 400.0, 800.0 };

            foreach (double Ratio in Ratios)
            {
                RatioCB.Items.Add(Math.Round(Ratio, 2) + "%");
            }

            this.Activate();
            this.Topmost = true;
        }

        private void Editor_RatioChanged()
        {
            WidthTB.Text = Math.Round((Editor.MainGrid.Width - 10.0),2).ToString();
            HeightTB.Text = Math.Round((Editor.MainGrid.Height - 10.0), 2).ToString();
        }

        private void Editor_ImageSizeChanged()
        {
            WidthTB.Text = Math.Round((Editor.MainGrid.Width - 10.0), 2).ToString();
            HeightTB.Text = Math.Round((Editor.MainGrid.Height - 10.0), 2).ToString();
        }

        private void EditBtnClick(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((System.Windows.Controls.RadioButton)sender).Tag.ToString());
            if (Index == 2) CropInfoTB.Visibility = Visibility.Visible;
            else CropInfoTB.Visibility = Visibility.Hidden;
            Editor.ImageEditMode = (EditMode)Index;
        }

        private void SelectionChanging(object sender, SelectionChangedEventArgs e)
        {
            if (RatioCB.SelectedItem == null) return;

            double Ratio = Convert.ToDouble(((string)RatioCB.SelectedItem).Substring(0, ((string)RatioCB.SelectedItem).Length - 1));

            Editor.Ratio = Ratio;
        }

        private void RatioChaning()
        {
            //RatioCB.SelectionChanged -= SelectionChanging;
            //RatioCB.Text = Math.Round((ImgEditor.Ratio * 100),2) + "%";
            //RatioCB.SelectionChanged += SelectionChanging;
        }

        private void ForDebugChangeSize(object sender, EventArgs e)
        {
            //WidthTB.Text = "Width : " + (int)ImgEditor.InnerImg.ActualWidth;
            //HeightTB.Text = "Height : " + (int)ImgEditor.InnerImg.ActualHeight;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Editor.ImageStretchMode = (Stretch)comboBox.SelectedIndex;
        }

        private void comboBox_Copy_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Editor.ImageEditMode = (UserControls.EditMode)comboBox_Copy.SelectedIndex;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "png 파일|*.png|jpg 파일|*.jpg";
            if (ofd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                //Editor.image = new BitmapImage(new Uri(ofd.FileName));
            }
        }

        private void button_Copy_Click(object sender, RoutedEventArgs e)
        {

        }
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetData(System.Windows.DataFormats.Bitmap, Editor.InnerImg.Source);
            
        }

        private void Editor_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl)) return;
            bool IsUp = e.Delta > 0 ? true : false;

            LimitInt li = new LimitInt(0, RatioCB.Items.Count - 1, RatioCB.SelectedIndex);

            if (IsUp) li.Value++;
            else li.Value--;

            RatioCB.SelectedIndex = li.Value;
        }
    }
}
