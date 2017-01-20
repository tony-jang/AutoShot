using AutoCapturer.Setting;
using AutoCapturer.UserControls;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using static AutoCapturer.Globals.Globals;

namespace AutoCapturer
{
    /// <summary>
    /// SettingWdw.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Windows.ChromeWindow
    {
        RadioButton[] AuCaRingTypeRB, GetImageRB, AllCaCountDownRB, PopCountDownRB;
        public SettingWindow()
        {
            InitializeComponent();

            ShowPopupGrid.IsEnabled = false;


            AuCaRingTypeRB = new RadioButton[]{ AuCaRingType1, AuCaRingType2, AuCaRingType3, AuCaRingType4 };

            GetImageRB = new RadioButton[]{ GetURLImage1, GetURLImage2, GetURLImage3,
                           GetTagImage1, GetTagImage2, GetTagImage3};
            AllCaCountDownRB = new RadioButton[]{ AllCaCountDown1, AllCaCountDown2, AllCaCountDown3,
                                 AllCaCountDown4, AllCaCountDown5, AllCaCountDown6 };
            PopCountDownRB = new RadioButton[]{ PopCountDown1, PopCountDown2, PopCountDown3 };

            AuCaRingType2.Unchecked += AuCaRing_Change;
            AuCaRingType3.Unchecked += AuCaRing_Change;
            foreach (RadioButton rb in AuCaRingTypeRB)
                rb.Checked += AuCaRing_Change;

            foreach (RadioButton rb in GetImageRB)
                rb.Checked += GetImage_Change;

            foreach (RadioButton rb in AllCaCountDownRB)
                rb.Checked += AllCaCountDown_Change;

            foreach (RadioButton rb in PopCountDownRB)
                rb.Checked += PopCount_Change;
            

            RecoWidthTB.PreviewTextInput += RecoRangeTBPreviewCheck;
            RecoHeightTB.PreviewTextInput += RecoRangeTBPreviewCheck;

            RecoWidthTB.TextChanged += RecoRangeTBChanged;
            RecoHeightTB.TextChanged += RecoRangeTBChanged;
        }

        public new void ShowDialog()
        {
            SettingSync(CurrentSetting);
            base.ShowDialog();
        }


        private void PopCount_Change(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            CurrentSetting.PopupCountSecond = Index;
        }

        private void GetImage_Change(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            CurrentSetting.ImageFromURLSave = (HowtoSaveGetPicture)(Index + 1);
        }

        private void AllCaCountDown_Change(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            CurrentSetting.AllCaptureCountDown = Index;
        }

        private void AuCaRing_Change(object sender, RoutedEventArgs e)
        {
            ShowPopupGrid.IsEnabled = ((bool)AuCaRingType2.IsChecked || (bool)AuCaRingType3.IsChecked);

            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            CurrentSetting.AutoCaptureEnableSelection = (AuCaEnableSelection)(Index + 1);
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
                    RecoWidthTB.Text = RecoWidth.ToString();
                    RecoHeightTB.Text = value.ToString();
                }

                MouseRangeRect.Height = value * 16;
            }
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

                ptnitm.SaveLocation = result.SaveLocation;
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
            PatternItem itm = (PatternItem)listView.SelectedItem;

            if (itm == null)
            {
                TBPtnName.Text = null;
                TBSaveLoc.Text = null;
                SettingGrid.IsEnabled = false;
            }
            else
            {
                SettingGrid.IsEnabled = true;
                TBPtnName.Text = itm.Content.ToString();
                TBSaveLoc.Text = itm.SaveLocation;
                SWOpenEditor.IsChecked = itm.pattern.OpenEffector;
                SWOverwrite.IsChecked = itm.pattern.OverWrite;
                SWPutLogo.IsChecked = itm.pattern.PutLogo;

            }
        }

        private void SWOpenEditor_Checked(object sender, RoutedEventArgs e)
        {
            ((PatternItem)listView.SelectedItem).pattern.OpenEffector = (bool)SWOpenEditor.IsChecked;
            CurrentSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }

        private void SWOverwrite_Checked(object sender, RoutedEventArgs e)
        {
            ((PatternItem)listView.SelectedItem).pattern.OverWrite = (bool)SWOverwrite.IsChecked;
            CurrentSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }

        private void SWPutLogo_Checked(object sender, RoutedEventArgs e)
        {
            ((PatternItem)listView.SelectedItem).pattern.PutLogo = (bool)SWPutLogo.IsChecked;
            CurrentSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }

        private void BtnAutoEffector_Click(object sender, RoutedEventArgs e)
        {
            // 자동 이펙터 설정
        }
        

        private void TBPtnName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;
            if (itm == null) return;

            itm.Content = TBPtnName.Text;
            ((PatternItem)listView.SelectedItem).pattern.PatternName = TBPtnName.Text;
            CurrentSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }

        private void TBSaveLoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;
            if (itm == null) return;

            itm.SaveLocation = TBSaveLoc.Text;
            ((PatternItem)listView.SelectedItem).pattern.SaveLocation = TBSaveLoc.Text;
            CurrentSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }



        #region [ Setting 설정에 맟추기 ]
        public void SettingSync(Setting.Setting setting)
        {
            #region [ 캡처 설정 ]
            foreach (RadioButton rb in AuCaRingTypeRB)
                if (int.Parse(rb.Tag.ToString()) + 1 == (int)setting.AutoCaptureEnableSelection) rb.IsChecked = true;

            foreach (RadioButton rb in PopCountDownRB)
                if (int.Parse(rb.Tag.ToString()) == setting.PopupCountSecond) rb.IsChecked = true;

            foreach (RadioButton rb in AllCaCountDownRB)
                if (int.Parse(rb.Tag.ToString()) == setting.AllCaptureCountDown) rb.IsChecked = true;

            foreach (RadioButton rb in new RadioButton[] { GetURLImage1, GetURLImage2, GetURLImage3 })
                if (int.Parse(rb.Tag.ToString()) + 1 == (int)setting.ImageFromURLSave) rb.IsChecked = true;

            foreach (RadioButton rb in new RadioButton[] { GetTagImage1, GetTagImage2, GetTagImage3 })
                if (int.Parse(rb.Tag.ToString()) + 1 == (int)setting.ImageFromImageTag) rb.IsChecked = true;
            #endregion

            #region [ 패턴 관리 ]
            listView.Items.Clear();
            int i = 0;
            foreach(SavePattern ptn in new SavePattern[] { CurrentSetting.DefaultPattern })
            {
                listView.Items.Add(new PatternItem(ptn.SaveLocation, ptn.PatternName, ptn));
                if (ReferenceEquals(ptn, setting.DefaultPattern))
                {
                    listView.SelectedIndex = i;
                }
                i++;
            }
            TBPtnCount.Text = $"등록된 패턴 ({listView.Items.Count}개)";
            #endregion
        }
        #endregion
    }
}