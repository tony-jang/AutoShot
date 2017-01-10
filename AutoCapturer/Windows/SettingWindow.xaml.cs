using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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

            RecoWidthTB.PreviewTextInput += RecoRangeTBPreviewCheck;
            RecoHeightTB.PreviewTextInput += RecoRangeTBPreviewCheck;

            RecoWidthTB.TextChanged += RecoRangeTBChanged;
            RecoHeightTB.TextChanged += RecoRangeTBChanged;

        }

        private void RecoRangeTBChanged(object sender, TextChangedEventArgs e)
        {
            TextBox tb = (TextBox)sender;
            string t = tb.Name.Substring(4, 1);
            try
            {
                switch (t)
                {
                    case "W":
                        RecoWidth = int.Parse(tb.Text);
                        break;
                    case "H":
                        RecoHeight = int.Parse(tb.Text);
                        break;
                }
            }
            catch (FormatException)
            {
                switch (t)
                {
                    case "W":
                        RecoWidth = 1;
                        break;
                    case "H":
                        RecoHeight = 1;
                        break;
                }
            }
        }

        private void RecoRangeTBPreviewCheck(object sender, TextCompositionEventArgs e)
        {
            e.Handled = IsTextAllowed(e.Text);
        }


        // 환경 설정

        // └ 마우스 인식 범위

        private static bool IsTextAllowed(string text)
        {
            Regex reg = new Regex("[^0-9]");
            return reg.IsMatch(text);
        }

        public int RecoWidth
        {
            get { return (int)(MouseRangeRect.Width / 16); }
            set
            {
                bool f = false;
                if (value <= 0) { value = 1; f = true; }
                else if (value > 10) { value = 10; f = true; }
                if (f)
                {
                    RecoWidthTB.Text = value.ToString();
                    RecoHeightTB.Text = RecoHeight.ToString();
                }

                MouseRangeRect.Width = value * 16;
            }
        }

        public int RecoHeight
        {
            get { return (int)(MouseRangeRect.Height / 16); }
            set
            {
                bool f = false;
                if (value <= 0) { value = 1; f = true; }
                else if (value > 10) { value = 10; f = true; }
                if (f)
                {
                    RecoWidthTB.Text = RecoHeight.ToString();
                    RecoHeightTB.Text = value.ToString();
                }

                MouseRangeRect.Height = value * 16;
            }
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
            if (itm == null) return;

            itm.Content = TBPtnName.Text;
        }

        private void TBSaveLoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;
            if (itm == null) return;

            itm.SavePattern = TBSaveLoc.Text;
        }



        
    }
}