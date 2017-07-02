using AutoShot.Setting;
using AutoShot.UserControls;
using AutoShot.Windows;
using AutoShot.Worker;
using Microsoft.Win32;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using static AutoShot.Globals.Globals;

namespace AutoShot
{
    /// <summary>
    /// SettingWdw.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SettingWindow : Windows.ChromeWindow
    {
        RadioButton[] AuCaRingTypeRB, AllCaCountDownRB, PopCountDownRB;
        RadioButton[] GetImageRB;
        ShortcutKey[] shortcutKeys;

        Setting.Setting TempSetting;

        public SettingWindow()
        {
            InitializeComponent();

            ShowPopupGrid.IsEnabled = false;
            
            AuCaRingTypeRB = new RadioButton[]{ AuCaRingType1, AuCaRingType2, AuCaRingType3, AuCaRingType4 };

            GetImageRB = new RadioButton[] { GetURLImage1, GetURLImage2, GetURLImage3 };
            AllCaCountDownRB = new RadioButton[]{ AllCaCountDown1, AllCaCountDown2, AllCaCountDown3,
                                 AllCaCountDown4, AllCaCountDown5, AllCaCountDown6 };
            PopCountDownRB = new RadioButton[] { PopCountDown1, PopCountDown2, PopCountDown3 };
            shortcutKeys = new ShortcutKey[] { scAuto, scAll, scSelect, scSetting, scEditMode, };

            AuCaRingType2.Unchecked += AuCaRing_Change;
            AuCaRingType3.Unchecked += AuCaRing_Change;

            foreach (RadioButton rb in AuCaRingTypeRB)
                rb.Checked += AuCaRing_Change;
            foreach (RadioButton rb in GetImageRB)
                rb.Checked += GetURLImage_Change;
            foreach (RadioButton rb in AllCaCountDownRB)
                rb.Checked += AllCaCountDown_Change;
            foreach (RadioButton rb in PopCountDownRB)
                rb.Checked += PopCount_Change;

            this.Closing += SettingWindow_Closing;

            RecoWidthTB.PreviewTextInput += RecoRangeTBPreviewCheck;
            RecoHeightTB.PreviewTextInput += RecoRangeTBPreviewCheck;

            RecoWidthTB.LostFocus += RecoRangeTBChanged;
            RecoHeightTB.LostFocus += RecoRangeTBChanged;
        }

        CloseType SaveChangeAllow = CloseType.JustClose;
        private void SettingWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            TempSetting?.OnSettingChange();

            if (SaveChangeAllow == CloseType.Message)
            {
                MessageBoxResult result = MsgBox("이대로 나가면 변경 사항이 저장되지 않습니다.\n그래도 나가시겠습니까?", "저장 여부 확인", Globals.MessageBoxStyle.YesNo);
                if (result == MessageBoxResult.Yes)
                {
                    return;
                }
                else if (result == MessageBoxResult.No)
                {
                    e.Cancel = true;
                }
            }
            if (SaveChangeAllow == CloseType.SaveAndClose) CurrentSetting = (Setting.Setting)TempSetting.Clone();
        }

        string NonUse = "(사용하지 않음)";
        string[] names = { "BtnAutoSave", "BtnAllCapture", "BtnSelCapture", "BtnOpenSetting", "BtnChangeEditor" };
        Shotcut[] keys;

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            if (keys == null) keys = new Shotcut[] { TempSetting.AutoCaptureKey, TempSetting.AllCaptureKey,
                TempSetting.SelectCaptureKey, TempSetting.OpenSettingKey, TempSetting.ChangeEditorModeKey };

            Key key = ShowKeyInput((Key)(((Button)sender).Tag));

            if (key == Key.None) ((Button)sender).Content = NonUse;
            else ((Button)sender).Content = key + " Key";

            ((Button)sender).Tag = (int)key;

            string basename = ((Button)sender).Name.Substring(0, ((Button)sender).Name.Length - 1);
            int basenum = int.Parse(((Button)sender).Name.Substring(((Button)sender).Name.Length - 1));
            
            

            foreach (Shotcut k in keys)
            {
                k.IsDisabled = false;
            }

            for(int i = 0; i <= 4; i++)
            {
                Button Btn1 = (Button)FindName(names[i] + "1");
                Button Btn2 = (Button)FindName(names[i] + "2");
                TextBlock TB = (TextBlock)FindName(names[i] + "lbl");
                if (Btn1.Content.ToString() == Btn2.Content.ToString()) TB.Foreground = Brushes.Green; else TB.Foreground = Brushes.Black;

                if (Btn1.Content.ToString() == NonUse && Btn2.Content.ToString() == NonUse) TB.Foreground = Brushes.Red;
            }

            for(int i = 0; i <= 4; i++)
            {
                Button iBtn1, iBtn2,  jBtn1, jBtn2;
                TextBlock ilbl, jlbl;

                iBtn1 = (Button)FindName(names[i] + "1");
                iBtn2 = (Button)FindName(names[i] + "2");
                ilbl = (TextBlock)FindName(names[i] + "lbl");
                for (int j= i + 1; j <= 4; j++)
                {
                    jBtn1 = (Button)FindName(names[j] + "1");
                    jBtn2 = (Button)FindName(names[j] + "2");
                    jlbl = (TextBlock)FindName(names[j] + "lbl");

                    if ((iBtn1.Content.ToString() == jBtn1.Content.ToString() && iBtn2.Content.ToString() == jBtn2.Content.ToString()) ||
                        (iBtn1.Content.ToString() == jBtn2.Content.ToString() && iBtn2.Content.ToString() == jBtn1.Content.ToString()))
                    {
                        keys[i].IsDisabled = true;
                        keys[j].IsDisabled = true;
                        ilbl.Foreground = Brushes.Red;
                        jlbl.Foreground = Brushes.Red;
                    }   
                }
            }
            
            //switch (basename)
            //{
            //    case "BtnAutoSave":
            //        if (basenum == 1) TempSetting.AutoCaptureKey.FirstKey = key; else TempSetting.AutoCaptureKey.SecondKey = key;
            //        break;
            //    case "BtnAllCapture":
            //        if (basenum == 1) TempSetting.AllCaptureKey.FirstKey = key; else TempSetting.AllCaptureKey.SecondKey = key;
            //        break;
            //    case "BtnSelCapture":
            //        if (basenum == 1) TempSetting.SelectCaptureKey.FirstKey = key; else TempSetting.SelectCaptureKey.SecondKey = key;
            //        break;
            //    case "BtnOpenSetting":
            //        if (basenum == 1) TempSetting.OpenSettingKey.FirstKey = key; else TempSetting.OpenSettingKey.SecondKey = key;
            //        break;
            //    case "BtnChangeEditor":
            //        if (basenum == 1) TempSetting.ChangeEditorModeKey.FirstKey = key; else TempSetting.ChangeEditorModeKey.SecondKey = key;
            //        break;
            //}
        }

        public new void ShowDialog()
        {
            TempSetting = (Setting.Setting)CurrentSetting.Clone();
            SettingSync(TempSetting);
            base.ShowDialog();
        }


        private void PopCount_Change(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            TempSetting.PopupCountSecond = Index;
        }


        private void GetTagImage_Change(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            TempSetting.ImageFromImageTag = (ImageSaveMode)(Index + 1);
        }
        private void GetURLImage_Change(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            TempSetting.ImageFromURLSave = (ImageSaveMode)(Index + 1);
        }

        private void AllCaCountDown_Change(object sender, RoutedEventArgs e)
        {
            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            TempSetting.AllCaptureCountDown = Index;
        }

        private void AuCaRing_Change(object sender, RoutedEventArgs e)
        {
            ShowPopupGrid.IsEnabled = ((bool)AuCaRingType2.IsChecked || (bool)AuCaRingType3.IsChecked);

            int Index = int.Parse(((RadioButton)sender).Tag.ToString());

            TempSetting.AutoCaptureEnableSelection = (CaptureNotifionMode)(Index + 1);
        }

        private void RecoRangeTBChanged(object sender, EventArgs e)
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
                TempSetting.RecoWidth = value;
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
                TempSetting.RecoHeight = value;
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
            PatternWdw ptnwdw = new PatternWdw();

            PatternInputResult result = ptnwdw.ShowDialog();

            if (result.IsSuccessfulReturn)
            {
                var ptnitm = new PatternItem();
                var sp = new SavePattern(result.Pattern, result.SaveLocation);
                ptnitm.SaveLocation = result.SaveLocation;
                ptnitm.Content = result.Pattern;
                ptnitm.pattern = sp;
                listView.Items.Add(ptnitm);
                TempSetting.Patterns.Add(sp);
            }
            
            TBPtnCount.Text = $"등록된 패턴 ({listView.Items.Count}개)";
            if (listView.SelectedIndex == -1) listView.SelectedIndex = 0;
        }

        private void PtnRemBtn_Click(object sender, RoutedEventArgs e)
        {
            bool ChangeDefault = false;
            if (listView.Items.Count == 1) { MsgBox("최소 패턴이 1개 이상은 존재해야 합니다!", "패턴 삭제 불가!"); return; }
            if (listView.SelectedItem != null)
            {
                if (MsgBox("정말 해당 패턴을 삭제하시겠습니까?", "삭제 확인", Globals.MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
                {
                    if (((PatternItem)listView.SelectedItem).pattern == TempSetting.DefaultPattern) ChangeDefault = true;
                    TempSetting.Patterns.Remove(((PatternItem)listView.SelectedItem).pattern);
                    listView.Items.Remove(listView.SelectedItem);
                }
                
            }


            if (ChangeDefault) ((PatternItem)listView.Items[0]).IsDefaultPattern = true;
            TempSetting.DefaultPatternIndex = 0;

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
                NonSelGrid.Visibility = Visibility.Visible;
            }
            else
            {
                if (SettingGrid == null) return;
                SettingGrid.IsEnabled = true;
                TBPtnName.Text = itm.Content.ToString();
                TBSaveLoc.Text = itm.SaveLocation;
                SWOpenEditor.IsChecked = itm.pattern.OpenEffector;
                SWOverwrite.IsChecked = itm.pattern.OverWrite;
                SWSaveImm.IsChecked = itm.pattern.SaveImmediately;
                BtnSetToDefaultPtn.IsEnabled = !itm.IsDefaultPattern;
                NonSelGrid.Visibility = Visibility.Hidden;

            }
        }

        private void SWOpenEditor_Checked(object sender, RoutedEventArgs e)
        {
            ((PatternItem)listView.SelectedItem).pattern.OpenEffector = (bool)SWOpenEditor.IsChecked;
            TempSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }

        private void SWOverwrite_Checked(object sender, RoutedEventArgs e)
        {
            ((PatternItem)listView.SelectedItem).pattern.OverWrite = (bool)SWOverwrite.IsChecked;
            TempSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }

        private void SWSaveImm_Checked(object sender, RoutedEventArgs e)
        {
            ((PatternItem)listView.SelectedItem).pattern.SaveImmediately = (bool)SWSaveImm.IsChecked;
            TempSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }


        private void BtnSetToDefaultPtn_Click(object sender, RoutedEventArgs e)
        {
            TempSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;

            int innerctr = 0;
            foreach (PatternItem itm in listView.Items)
            {
                if (innerctr == TempSetting.DefaultPatternIndex) itm.IsDefaultPattern = true;
                else itm.IsDefaultPattern = false;
                innerctr++;
            }

            BtnSetToDefaultPtn.IsEnabled = false;
        }
        private void TBPtnName_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;
            if (itm == null) return;

            itm.Content = TBPtnName.Text;
            ((PatternItem)listView.SelectedItem).pattern.PatternName = TBPtnName.Text;
            TempSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
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
                    SettingWriter settingw = new SettingWriter((Setting.Setting)CurrentSetting.Clone());
                    MsgBox("정상적으로 변경 완료되었습니다!");

                    SaveChangeAllow = CloseType.JustClose;
                    this.Close();
                }
                
            }
            catch (NullReferenceException)
            { }
            
        }

        private void BtnExport_Click(object sender, RoutedEventArgs e)
        {
            // 엑스포트 (내보내기) 비활성화된 기능
            var wdw = new FileSaveWindow();

            if (wdw.ShowDialog())
            {
                MsgBox("정상적으로 내보내기가 완료되었습니다.", "내보내기 완료");
            }
        }

        private void swStartupProgram_Checked(object sender, RoutedEventArgs e)
        {
            TempSetting.IsStartupProgram = (bool)swStartupProgram.IsChecked;
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var hw = new HelpWindow();

            hw.ShowDialog();
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            if (MsgBox("정말 초기화 하시겠습니까?", "초기화 여부 확인", Globals.MessageBoxStyle.YesNo) == MessageBoxResult.Yes)
            {
                Thread thr = new Thread(() => 
                {
                    if (File.Exists("setting.aucasetting"))
                    {
                        Dispatcher.Invoke(new Action(() => { this.IsEnabled = false; }));
                        File.Delete("setting.aucasetting");

                        RegistryKey rkey = Registry.CurrentUser.CreateSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run");
                        
                        rkey.DeleteValue("AutoShot", false);


                        Thread.Sleep(1000);
                        System.Environment.Exit(0);
                    }
                    else
                    {
                        MsgBox("초기화할 파일이 없습니다!", "파일 없음!");
                    }
                    
                });
                thr.SetApartmentState(ApartmentState.STA);
                thr.Start();
                
            }

        }

        private void BtnShowLog_Click(object sender, RoutedEventArgs e)
        {
            MsgBox("[ 업데이트 로그 ]\nVer 1.0 - AutoShot 개발 완료" + Environment.NewLine +
                "Ver 1.0.1 - 오류 수정 및 설정 불러오기/내보내기 비활성화" + Environment.NewLine +
                "Ver 1.0.2 - 오류 수정 (시작 프로그램 레지스트리 설정) 및 설정 불러오기 재활성화" + Environment.NewLine +
                "Ver 1.0.3 - 시스템 위치에 세팅 위치가 잡히는 오류 수정", "업데이트 로그");
        }

        public enum CloseType
        {
            SaveAndClose,
            Message,
            JustClose,
        }

        private void BtnApply_Click(object sender, RoutedEventArgs e)
        {
            SaveChangeAllow = CloseType.SaveAndClose;
            this.Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            SaveChangeAllow = CloseType.Message;
            this.Close();
        }

        private void btnSettingLocation_Click(object sender, RoutedEventArgs e)
        {
            var fi = ShowSelectFileDialog(new string[] { ".aucasetting" });
            SettingWriter sw = new SettingWriter(TempSetting, fi.FullName);
        }

        private void TBSaveLoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            UserControls.PatternItem itm = (UserControls.PatternItem)listView.SelectedItem;
            if (itm == null) return;

            itm.SaveLocation = TBSaveLoc.Text;
            ((PatternItem)listView.SelectedItem).pattern.SaveLocation = TBSaveLoc.Text;
            TempSetting.DefaultPattern = ((PatternItem)listView.SelectedItem).pattern;
        }



        #region [ Setting 설정에 맟추기 ]
        public void SettingSync(Setting.Setting setting)
        {
            if (keys == null) keys = new Shotcut[] { //CurrentSetting.AutoCaptureKey,
                CurrentSetting.AllCaptureKey,
                CurrentSetting.SelectCaptureKey, CurrentSetting.OpenSettingKey, CurrentSetting.ChangeEditorModeKey };

            #region [ 캡처 설정 ]
            foreach (RadioButton rb in AuCaRingTypeRB)
                if (int.Parse(rb.Tag.ToString()) + 1 == (int)setting.AutoCaptureEnableSelection) rb.IsChecked = true;

            foreach (RadioButton rb in PopCountDownRB)
                if (int.Parse(rb.Tag.ToString()) == setting.PopupCountSecond) rb.IsChecked = true;

            foreach (RadioButton rb in AllCaCountDownRB)
                if (int.Parse(rb.Tag.ToString()) == setting.AllCaptureCountDown) rb.IsChecked = true;

            foreach (RadioButton rb in new RadioButton[] { GetURLImage1, GetURLImage2, GetURLImage3 })
                if (int.Parse(rb.Tag.ToString()) + 1 == (int)setting.ImageFromURLSave) rb.IsChecked = true;
            
            #endregion

            #region [ 환경 설정 ]
            RecoHeight = CurrentSetting.RecoHeight;
            RecoWidth = CurrentSetting.RecoWidth;

            

                //for (int it = 0; it < 4; it++)
                //{
                //    Button Btn1 = (Button)FindName(names[it] + "1");
                //    Button Btn2 = (Button)FindName(names[it] + "2");
                //    TextBlock TB = (TextBlock)FindName(names[it] + "lbl");
                //    if (Btn1.Content.ToString() == Btn2.Content.ToString()) TB.Foreground = Brushes.Green; else TB.Foreground = Brushes.Black;

                //    if (Btn1.Content.ToString() == NonUse && Btn2.Content.ToString() == NonUse) TB.Foreground = Brushes.Red;
                //}

                //for (int it = 0; it < 4; it++)
                //{
                //    Button iBtn1, iBtn2, jBtn1, jBtn2;
                //    TextBlock ilbl, jlbl;

                //    iBtn1 = (Button)FindName(names[it] + "1");
                //    iBtn2 = (Button)FindName(names[it] + "2");
                //    ilbl = (TextBlock)FindName(names[it] + "lbl");
                //    for (int j = it + 1; j < 5; j++)
                //    {
                //        jBtn1 = (Button)FindName(names[j] + "1");
                //        jBtn2 = (Button)FindName(names[j] + "2");
                //        jlbl = (TextBlock)FindName(names[j] + "lbl");

                //        if ((iBtn1.Content.ToString() == jBtn1.Content.ToString() && iBtn2.Content.ToString() == jBtn2.Content.ToString()) ||
                //            (iBtn1.Content.ToString() == jBtn2.Content.ToString() && iBtn2.Content.ToString() == jBtn1.Content.ToString()))
                //        {
                //            keys[it].IsDisabled = true;
                //            keys[j].IsDisabled = true;
                //            ilbl.Foreground = Brushes.Red;
                //            jlbl.Foreground = Brushes.Red;
                //        }

                //    }
                //}

            swStartupProgram.IsChecked = setting.IsStartupProgram;

            #endregion

            #region [ 패턴 관리 ]
            listView.Items.Clear();
            int ctr = 0;
            foreach(SavePattern ptn in CurrentSetting.Patterns)
            {
                listView.Items.Add(new PatternItem(ptn.SaveLocation, ptn.PatternName, ptn));
                if (CurrentSetting.DefaultPattern == ptn)
                {
                    int innerctr = 0;
                    foreach (PatternItem itm in listView.Items)
                    {

                        if (innerctr == ctr) itm.IsDefaultPattern = true;
                        else itm.IsDefaultPattern = false;
                        innerctr++;
                    }
                    
                }
                ctr++;
            }
            TBPtnCount.Text = $"등록된 패턴 ({listView.Items.Count}개)";
            #endregion
        }
        #endregion
    }
}