using AutoCapturer.Setting;
using AutoCapturer.UserControls;
using System;
using System.Collections.Generic;
using System.IO;
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
            Windows.PopUpWindow.AllWindowClose();
            InitializeComponent();
            Editor.ImageEditMode = UserControls.EditMode.SizeChange;
            
            EditBtn1.Click += EditBtnClick;
            EditBtn2.Click += EditBtnClick;
            EditBtn3.Click += EditBtnClick;
            
            RatioCB.SelectionChanged += SelectionChanging;

            Editor.RatioChanged += Editor_RatioChanged;
            Editor.ImageSizeChanged += Editor_ImageSizeChanged;

            double[] Ratios = { 12.5, 25.0, 33.33, 50.0, 66.67, 100.0, 150.0, 200.0, 400.0, 800.0, 1600.0, 3200.0 };

            foreach (double Ratio in Ratios)
            {
                RatioCB.Items.Add(Math.Round(Ratio, 2) + "%");
            }

            this.Activate();
            
        }

        Tuple<bool, BitmapImage> returndata = new Tuple<bool, BitmapImage>(false, null);

        public new Tuple<bool, BitmapImage> ShowDialog()
        {
            base.ShowDialog();

            return returndata;
        }


        private void Editor_RatioChanged()
        {
            WidthTB.Text = Math.Round((Editor.MainGrid.Width - 10.0),2).ToString();
            HeightTB.Text = Math.Round((Editor.MainGrid.Height - 10.0), 2).ToString();

            double Ratio = Convert.ToDouble(((string)RatioCB.SelectedItem).Substring(0, ((string)RatioCB.SelectedItem).Length - 1));
            if (Ratio != Editor.Ratio) RatioCB.Text = Editor.Ratio + "%";

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
            //RenderTargetBitmap img = (RenderTargetBitmap)Editor.Originalimage;

            
        }

        private void SelectionChanging(object sender, SelectionChangedEventArgs e)
        {
            if (RatioCB.SelectedItem == null) return;

            double Ratio = Convert.ToDouble(((string)RatioCB.SelectedItem).Substring(0, ((string)RatioCB.SelectedItem).Length - 1));

            Editor.Ratio = Ratio;
        }

        public BitmapImage GetVisibleEditorImage()
        {
            BitmapImage img = new BitmapImage();

            img.BeginInit();
            PngBitmapEncoder enc = new PngBitmapEncoder();
            img.StreamSource = new MemoryStream(Globals.Globals.ImageSourceToBytes(enc, Editor.InnerImg.Source));

            img.CacheOption = BitmapCacheOption.OnLoad;
            img.DecodePixelWidth = (int)Editor.RealSize.X;
            img.DecodePixelHeight = (int)Editor.RealSize.Y;
            img.EndInit();

            return img;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            System.Windows.Clipboard.SetData(System.Windows.DataFormats.Bitmap, GetVisibleEditorImage());
        }
        

        private void Editor_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (!Keyboard.IsKeyDown(Key.LeftCtrl)) return;
            bool IsUp = e.Delta > 0 ? true : false;

            LimitInt li = new LimitInt(0, RatioCB.Items.Count - 1, RatioCB.SelectedIndex);

            if (IsUp) li.Value++;
            else li.Value--;

            RatioCB.SelectedIndex = li.Value;
            e.Handled = true;
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void BtnOriginSizeReturn_Click(object sender, RoutedEventArgs e)
        {
            Editor.ResetToOringinal();
        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            returndata = new Tuple<bool, BitmapImage>(true, GetVisibleEditorImage());
            this.Close();
        }
    }
}
