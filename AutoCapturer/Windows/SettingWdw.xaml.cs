using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
using System.Windows.Shell;

namespace AutoCapturer
{
    /// <summary>
    /// SettingWdw.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWdw : Windows.ChromeWindow
    {
        
        public SettingWdw()
        {
            
            InitializeComponent();

            ShowPopupGrid.IsEnabled = false;
            
            AuCaRingType2.Unchecked += PopupStateChange;
            AuCaRingType2.Checked += PopupStateChange;

            TBPtnName.TextChanged += TBPtnNamechange;
        }

        private void TBPtnNamechange(object sender, TextChangedEventArgs e)
        {
            string Data;

            Converter.VariableConverter.ConvertAll(TBPtnName.Text, out Data);

            PtnPreview.Text = Data;

            return;

        }

        private void PopupStateChange(object sender, RoutedEventArgs e)
        {
            ShowPopupGrid.IsEnabled = (bool)AuCaRingType2.IsChecked;
        }

        void DragBrder_MD(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }
        
        private void CloseBtn_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Environment.Exit(0);
        }

        private void slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            OpacityFullScr.Opacity = 1 - OpacityFullScr.Value / 100;
        }

        private void EndButt_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }
    }
}