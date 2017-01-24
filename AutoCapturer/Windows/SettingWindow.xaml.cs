using AutoCapturer.Setting;
using AutoCapturer.UserControls;
using AutoCapturer.Windows;
using AutoCapturer.Worker;
using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static AutoCapturer.Globals.Globals;

namespace AutoCapturer
{
    /// <summary>
    /// SettingWdw.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Windows.ChromeWindow
    {
        RadioButton[] AuCaRingTypeRB, AllCaCountDownRB, PopCountDownRB;
        RadioButton[][] GetImageRB;
        Button[] ShortCutButtons;
        public SettingWindow()
        {
            InitializeComponent();

            ShowPopupGrid.IsEnabled = false;


            AuCaRingTypeRB = new RadioButton[]{ AuCaRingType1, AuCaRingType2, AuCaRingType3, AuCaRingType4 };

            GetImageRB = new RadioButton[][] { new RadioButton[] { GetURLImage1, GetURLImage2, GetURLImage3 },
                           new RadioButton[]{ GetTagImage1, GetTagImage2, GetTagImage3} };
            AllCaCountDownRB = new RadioButton[]{ AllCaCountDown1, AllCaCountDown2, AllCaCountDown3,
                                 AllCaCountDown4, AllCaCountDown5, AllCaCountDown6 };
            PopCountDownRB = new RadioButton[]{ PopCountDown1, PopCountDown2, PopCountDown3 };

            ShortCutButtons = new Button[] { BtnAutoSave1, BtnAutoSave2,
                BtnAllCapture1, BtnAllCapture2,
                BtnOpenSetting1, BtnOpenSetting2,
                BtnSelCapture1, BtnSelCapture2 };

            AuCaRingType2.Unchecked += AuCaRing_Change;
            AuCaRingType3.Unchecked += AuCaRing_Change;
            foreach (RadioButton rb in AuCaRingTypeRB)
                rb.Checked += AuCaRing_Change;

            foreach (RadioButton rb in GetImageRB[0])
            {
                rb.Checked += GetURLImage_Change;
            }

            foreach (RadioButton rb in GetImageRB[1])
            {
                rb.Checked += GetTagImage_Change;
            }

            foreach (RadioButton rb in AllCaCountDownRB)
                rb.Checked += AllCaCountDown_Change;

            foreach (RadioButton rb in PopCountDownRB)
                rb.Checked += PopCount_Change;

            foreach (Button btn in ShortCutButtons)
            {
                btn.Click += Btn_Click;
            }
            

            RecoWidthTB.PreviewTextInput += RecoRangeTBPreviewCheck;
            RecoHeightTB.PreviewTextInput += RecoRangeTBPreviewCheck;

            RecoWidthTB.TextChanged += RecoRangeTBChanged;
            RecoHeightTB.TextChanged += RecoRangeTBChanged;
            
        }


        string NonUse = "(사용하지 않음)";
        string[] names = { "BtnAutoSave", "BtnAllCapture", "BtnSelCapture", "BtnOpenSetting" };
        ShortCutKey[] keys;

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (keys == null) keys = new ShortCutKey[] { CurrentSetting.AutoCaptureKey, CurrentSetting.AllCaptureKey, CurrentSetting.SelectCaptureKey, CurrentSetting.OpenSettingKey };

            Key key = ShowKeyInput((Key)(((Button)sender).Tag));

            if (key == Key.None) ((Button)sender).Content = NonUse;
            else ((Button)sender).Content = key + " Key";

            ((Button)sender).Tag = (int)key;

            string basename = ((Button)sender).Name.Substring(0, ((Button)sender).Name.Length - 1);
            int basenum = int.Parse(((Button)sender).Name.Substring(((Button)sender).Name.Length - 1));
            
            

            foreach (ShortCutKey k in keys)
            {
                k.IsDisabled = false;
            }

            for(int i =0;i< 4; i++)
            {
                Button Btn1 = (Button)FindName(names[i] + "1");
                Button Btn2 = (Button)FindName(names[i] + "2");
                TextBlock TB = (TextBlock)FindName(names[i] + "lbl");
                if (Btn1.Content.ToString() == Btn2.Content.ToString()) TB.Foreground = Brushes.Green; else TB.Foreground = Brushes.Black;

                if (Btn1.Content.ToString() == NonUse && Btn2.Content.ToString() == NonUse) TB.Foreground = Brushes.Red;
            }

            for(int i = 0; i < 3; i++)
            {


                Button iBtn1, iBtn2,  jBtn1, jBtn2;
                TextBlock ilbl, jlbl;

                iBtn1 = (Button)FindName(names[i] + "1");
                iBtn2 = (Button)FindName(names[i] + "2");
                ilbl = (TextBlock)FindName(names[i] + "lbl");
                for (int j= i + 1; j < 4; j++)
                {
                    jBtn1 = (Button)FindName(names[j] + "1");
                    jBtn2 = (Button)FindName(names[j] + "2");
                    jlbl = (TextBlock)FindName(names[j] + "lbl");

                    if (iBtn1.Content.ToString() == jBtn1.Content.ToString() && iBtn2.Content.ToString() == jBtn2.Content.ToString())
                    {
                        keys[i].IsDisabled = true;
                        keys[j].IsDisabled = true;
                        ilbl.Foreground = Brushes.Red;
                        jlbl.Foreground = Brushes.Red;
                    }

                }
            }


            switch (basename)
            {
                case "BtnAutoSave":
                    if (basenum == 1) CurrentSetting.AutoCaptureKey.FirstKey = key; else CurrentSetting.AutoCaptureKey.SecondKey = key;
                    break;
                case "BtnAllCapture":
                    if (basenum == 1) CurrentSetting.AllCaptureKey.FirstKey = key; else CurrentSetting.AllCaptureKey.SecondKey = key;
                    break;
                case "BtnSelCapture":
                    if (basenum == 1) CurrentSetting.SelectCaptureKey.FirstKey = key; else CurrentSetting.SelectCaptureKey.SecondKey = key;
                    break;
                case "BtnOpenSetting":
                    if (basenum == 1) CurrentSetting.OpenSettingKey.FirstKey = key; else CurrentSetting.OpenSettingKey.SecondKey = key;
                    break;
            }
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


        private void GetTagImage_Change(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            CurrentSetting.ImageFromImageTag = (HowtoSaveGetPicture)(Index + 1);
        }
        private void GetURLImage_Change(object sender, RoutedEventArgs e)
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
                RecoWidthTB.Text = value.ToString();
                CurrentSetting.RecoWidth = value;
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
                RecoHeightTB.Text = value.ToString();
                CurrentSetting.RecoHeight = value;
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
                if (MsgBox("정말 해당 패턴을 삭제하시겠습니까?", "삭제 확인", Globals.MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
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
                BtnSetToDefaultPtn.IsEnabled = !itm.IsDefaultPattern;
                

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

        private void BtnSetToDefaultPtn_Click(object sender, RoutedEventArgs e)
        {
            ((PatternItem)listView.SelectedItem).IsDefaultPattern = true;
            BtnSetToDefaultPtn.IsEnabled = false;
        }
        private void TBPtnName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;
            if (itm == null) return;

            itm.Content = TBPtnName.Text;
            ((PatternItem)listView.SelectedItem).pattern.PatternName = TBPtnName.Text;
            CurrentSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }

        private void BtnImport_Click(object sender, RoutedEventArgs e)
        {
            // 임포트 (불러오기)
            try
            {
                SettingReader sr = new SettingReader(ShowSelectFileDialog(new string[] { ".aucasetting" }).FullName);
                if (MsgBox("정말 현재 설정을 바꾸시겠습니까?","설정 변경 여부 확인", Globals.MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
                {
                    CurrentSetting = sr.ReadSetting();
                    MsgBox("정상적으로 변경 완료되었습니다!");
                }
                
            }
            catch (NullReferenceException)
            { }
            
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            // 엑스포트 (내보내기)
            var wdw = new FileSaveWindow();

            if (wdw.ShowDialog())
            {
                MsgBox("정상적으로 내보내기가 완료되었습니다.", "내보내기 완료");
            }
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
            if (keys == null) keys = new ShortCutKey[] { CurrentSetting.AutoCaptureKey, CurrentSetting.AllCaptureKey, CurrentSetting.SelectCaptureKey, CurrentSetting.OpenSettingKey };

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

            #region [ 환경 설정 ]
            RecoHeight = CurrentSetting.RecoHeight;
            RecoWidth = CurrentSetting.RecoWidth;

            Button[][] btns = { new Button[]{ BtnAutoSave1, BtnAutoSave2 },
                                new Button[]{ BtnAllCapture1, BtnAllCapture2 },                 
                                new Button[]{ BtnSelCapture1, BtnSelCapture2 }, 
                                new Button[]{ BtnOpenSetting1, BtnOpenSetting2 } };

            int counter = 0;
            foreach(ShortCutKey k in keys)
            {
                btns[counter][0].Tag = (int)k.FirstKey;
                btns[counter][1].Tag = (int)k.SecondKey;

                btns[counter][0].Content = k.FirstKey.ToString() + " Key";
                btns[counter][1].Content = k.SecondKey.ToString() + " Key";
                counter++;
            }

            for (int it = 0; it < 4; it++)
            {
                Button Btn1 = (Button)FindName(names[it] + "1");
                Button Btn2 = (Button)FindName(names[it] + "2");
                TextBlock TB = (TextBlock)FindName(names[it] + "lbl");
                if (Btn1.Content.ToString() == Btn2.Content.ToString()) TB.Foreground = Brushes.Green; else TB.Foreground = Brushes.Black;

                if (Btn1.Content.ToString() == NonUse && Btn2.Content.ToString() == NonUse) TB.Foreground = Brushes.Red;
            }

            for (int it = 0; it < 3; it++)
            {
                Button iBtn1, iBtn2, jBtn1, jBtn2;
                TextBlock ilbl, jlbl;

                iBtn1 = (Button)FindName(names[it] + "1");
                iBtn2 = (Button)FindName(names[it] + "2");
                ilbl = (TextBlock)FindName(names[it] + "lbl");
                for (int j = it + 1; j < 4; j++)
                {
                    jBtn1 = (Button)FindName(names[j] + "1");
                    jBtn2 = (Button)FindName(names[j] + "2");
                    jlbl = (TextBlock)FindName(names[j] + "lbl");

                    if (iBtn1.Content.ToString() == jBtn1.Content.ToString() && iBtn2.Content.ToString() == jBtn2.Content.ToString())
                    {
                        keys[it].IsDisabled = true;
                        keys[j].IsDisabled = true;
                        ilbl.Foreground = Brushes.Red;
                        jlbl.Foreground = Brushes.Red;
                    }

                }
            }

            #endregion

            #region [ 패턴 관리 ]
            listView.Items.Clear();
            foreach(SavePattern ptn in new SavePattern[] { CurrentSetting.DefaultPattern })
            {
                listView.Items.Add(new PatternItem(ptn.SaveLocation, ptn.PatternName, ptn));
            }
            TBPtnCount.Text = $"등록된 패턴 ({listView.Items.Count}개)";
            #endregion
        }
        #endregion
    }
}