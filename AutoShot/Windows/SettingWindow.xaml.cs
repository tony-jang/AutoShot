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
        RadioButton[] auCaRingTypeRB, allCaCountDownRB, popCountDownRB;
        RadioButton[] getImageRB;
        ShortcutKey[] shortcutKeys;

        Setting.Setting TempSetting;

        public SettingWindow()
        {
            InitializeComponent();

            ShowPopupGrid.IsEnabled = false;
            
            auCaRingTypeRB = new RadioButton[]{ auCaRingType1, auCaRingType2, auCaRingType3, auCaRingType4 };

            getImageRB = new RadioButton[] { getURLImage1, getURLImage2, getURLImage3 };
            allCaCountDownRB = new RadioButton[]{ allCaCountDown1, allCaCountDown2, allCaCountDown3,
                                 allCaCountDown4, allCaCountDown5, allCaCountDown6 };
            popCountDownRB = new RadioButton[] { popCountDown1, popCountDown2, popCountDown3 };
            shortcutKeys = new ShortcutKey[] { scAuto, scAll, scSelect, scSetting, scEditMode, };

            auCaRingType2.Unchecked += AuCaRing_Change;
            auCaRingType3.Unchecked += AuCaRing_Change;

            foreach (RadioButton rb in auCaRingTypeRB)
                rb.Checked += AuCaRing_Change;
            foreach (RadioButton rb in getImageRB)
                rb.Checked += GetURLImage_Change;
            foreach (RadioButton rb in allCaCountDownRB)
                rb.Checked += AllCaCountDown_Change;
            foreach (RadioButton rb in popCountDownRB)
                rb.Checked += PopCount_Change;

            foreach (ShortcutKey sc in shortcutKeys)
                sc.KeyChanged += sc_KeyChanged;

            this.Closing += SettingWindow_Closing;

            RecoWidthTB.PreviewTextInput += RecoRangeTBPreviewCheck;
            RecoHeightTB.PreviewTextInput += RecoRangeTBPreviewCheck;

            RecoWidthTB.LostFocus += RecoRangeTBChanged;
            RecoHeightTB.LostFocus += RecoRangeTBChanged;
        }

        private void sc_KeyChanged(object sender, EventArgs e)
        {
            Shortcut[] shortcuts = { TempSetting.AllCaptureKey,
                                     TempSetting.AutoCaptureKey,
                                     TempSetting.ChangeEditorModeKey,
                                     TempSetting.SelectCaptureKey,
                                     TempSetting.OpenSettingKey };

            int i = 0;
            foreach (ShortcutKey scKey in new ShortcutKey[] { scAll, scAuto, scEditMode, scSelect, scSetting })
            {
                shortcuts[i].WPFKey = scKey.Key;

                shortcuts[i].Control = scKey.Control;
                shortcuts[i].Alt = scKey.Alt;
                shortcuts[i].Shift = scKey.Shift;
                i++;
            }
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
            ShowPopupGrid.IsEnabled = ((bool)auCaRingType2.IsChecked || (bool)auCaRingType3.IsChecked);

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

        Shortcut[] keys;

        #region [ Setting 설정에 맟추기 ]
        public void SettingSync(Setting.Setting setting)
        {
            if (keys == null) keys = new Shortcut[] { //CurrentSetting.AutoCaptureKey,
                CurrentSetting.AllCaptureKey,
                CurrentSetting.SelectCaptureKey, CurrentSetting.OpenSettingKey, CurrentSetting.ChangeEditorModeKey };

            #region [ 캡처 설정 ]
            foreach (RadioButton rb in auCaRingTypeRB)
                if (int.Parse(rb.Tag.ToString()) + 1 == (int)setting.AutoCaptureEnableSelection) rb.IsChecked = true;

            foreach (RadioButton rb in popCountDownRB)
                if (int.Parse(rb.Tag.ToString()) == setting.PopupCountSecond) rb.IsChecked = true;

            foreach (RadioButton rb in allCaCountDownRB)
                if (int.Parse(rb.Tag.ToString()) == setting.AllCaptureCountDown) rb.IsChecked = true;

            foreach (RadioButton rb in new RadioButton[] { getURLImage1, getURLImage2, getURLImage3 })
                if (int.Parse(rb.Tag.ToString()) + 1 == (int)setting.ImageFromURLSave) rb.IsChecked = true;
            
            #endregion

            #region [ 환경 설정 ]
            RecoHeight = CurrentSetting.RecoHeight;
            RecoWidth = CurrentSetting.RecoWidth;



            Shortcut[] shortcuts = { setting.AllCaptureKey,
                                     setting.AutoCaptureKey,
                                     setting.ChangeEditorModeKey,
                                     setting.SelectCaptureKey,
                                     setting.OpenSettingKey };

            int i = 0;
            foreach (ShortcutKey scKey in new ShortcutKey[]{ scAll, scAuto, scEditMode, scSelect, scSetting })
            {
                scKey.InitalizeData(shortcuts[i].WPFKey, shortcuts[i].Control, shortcuts[i].Alt, shortcuts[i].Shift);
                i++;
            }

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