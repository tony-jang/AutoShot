using AutoCapturer.Converter;
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
using static AutoCapturer.Converter.PatternConverter;
using static AutoCapturer.Globals.Globals;

namespace AutoCapturer.Windows
{
    /// <summary>
    /// PatternWdw.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class PatternWdw : ChromeWindow
    {
        System.Windows.Forms.Timer tmr = new System.Windows.Forms.Timer();
        string Ptn, Loc;

        public PatternWdw()
        {
            InitializeComponent();

            this.Closing += Frm_Closing;

            tmr.Interval = 100;
            tmr.Tick += PtnCheck;
            tmr.Start();
        }

        private void Frm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!IsApply) ReturnFlag = false;
        }

        public bool ReturnFlag = false, IsApply = false;
        
        public new PatternInputResult ShowDialog()
        {
            base.ShowDialog();

            return new PatternInputResult(TBPtnName.Text,TBSaveLoc.Text, ReturnFlag);

        }

        private void ApplyBtn_Click(object sender, RoutedEventArgs e)
        {
            IsApply = true;
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TBSaveLoc.Text = ShowSelectDirectoryDialog()?.FullName;
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void PtnCheck(object sender, EventArgs e)
        {
            bool PtnFlag = false, LocFlag = false;
            switch (ConvertAll(TBPtnName.Text, out Ptn))
            {
                case ErrorList.AlreadyAutoUsed:
                    PtnFlag = true;
                    TBPtnPreview.Text = "이미 자동 변수가 사용되었습니다.";
                    break;

                case ErrorList.CannotAccessString:
                    PtnFlag = false;
                    TBPtnPreview.Text = "파일명에 사용하지 못하는 문자가 포함되어 있습니다.";
                    break;

                case ErrorList.UnknownVariable:
                    PtnFlag = false;
                    TBPtnPreview.Text = "알 수 없는 변수가 사용되었습니다.";
                    break;

                case ErrorList.BlankString:
                    PtnFlag = false;
                    TBPtnPreview.Text = "빈 이름은 사용할 수 없습니다.";
                    break;

                case ErrorList.NoError:
                    PtnFlag = true;
                    TBPtnPreview.Text = Ptn;
                    break;
            }

            if (PtnFlag) TBPtnPreview.ToolTip = (new ToolTip().Content = Ptn);
            else TBPtnPreview.ToolTip = null;

            switch (LocationConverter.ConvertAll(TBSaveLoc.Text, out Loc))
            {
                case ErrorList.CannotAccessString:
                    TBLocPreview.Text = "파일명에 사용하지 못하는 문자가 포함되어 있습니다.";
                    LocFlag = false;
                    break;

                case ErrorList.UnknownVariable:
                    LocFlag = false;
                    break;
                case ErrorList.NoError:
                    TBLocPreview.Text = Loc;
                    LocFlag = true;
                    break;
            }

            if (LocFlag) TBLocPreview.ToolTip = (new ToolTip().Content = Loc); else TBLocPreview.ToolTip = null;

            if (PtnFlag && LocFlag) ReturnFlag = true; else ReturnFlag = false;

        }

    }

    public struct PatternInputResult
    {
        public PatternInputResult(string pattern, string saveloc, bool ReturnFlag)
        {
            Pattern = pattern;
            SaveLocation = saveloc;
            IsSuccessfulReturn = ReturnFlag;
        }
        public string Pattern;
        public string SaveLocation;
        public bool IsSuccessfulReturn;
    }


}
