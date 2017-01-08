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

        private void EndButt_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
        }

        private void PtnAddBtn_Click(object sender, RoutedEventArgs e)
        {
            Windows.PatternWdw ptnwdw = new Windows.PatternWdw();

            Windows.PatternInputResult result = ptnwdw.ShowDialog();

            if (result.IsSuccessfulReturn)
            {
                var ptnitm = new UserControls.PatternItem();

                ptnitm.SavePattern = result.SaveLocation;
                ptnitm.Content = result.Pattern;

                listView.Items.Add(ptnitm);
            }
            TBPtnCount.Text = $"등록된 패턴 ({listView.Items.Count}개)";
        }

        private void PtnRemBtn_Click(object sender, RoutedEventArgs e)
        {
            if (listView.SelectedItem != null)
            {
                if (MessageBox.Show("정말 해당 패턴을 삭제하시겠습니까?", "삭제 확인", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    listView.Items.Remove(listView.SelectedItem);
            }
            TBPtnCount.Text = $"등록된 패턴 ({listView.Items.Count}개)";

        }

        private void listView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;

            if (itm == null)
            {
                TBPtnName.Text = null; TBPtnName.IsEnabled = false;
                TBSaveLoc.Text = null; TBSaveLoc.IsEnabled = false;
            }
            else
            {
                TBPtnName.Text = itm.Content.ToString(); TBPtnName.IsEnabled = true;
                TBSaveLoc.Text = itm.SavePattern;        TBSaveLoc.IsEnabled = true;
            }
            TBPtnCount.Text = $"등록된 패턴 ({listView.Items.Count}개)";
        }

        private void TBPtnName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;

            itm.Content = TBPtnName.Text;
        }

        private void TBSaveLoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;

            itm.SavePattern = TBSaveLoc.Text;
        }
    }
}