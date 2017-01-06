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

            tmr.Interval = 100;
            tmr.Tick += PtnCheck;
            tmr.Start();
        }

        private void PtnCheck(object sender, EventArgs e)
        {
            bool flag = false;
            switch (PatternConverter.ConvertAll(TBPtnName.Text, out Ptn))
            {
                case ErrorList.AlreadyAutoUsed:
                    flag = true;
                    TBPtnPreview.Text = "이미 자동 변수가 사용되었습니다.";
                    break;

                case ErrorList.CannotAccessString:
                    flag = true;
                    TBPtnPreview.Text = "파일명에 사용하지 못하는 문자가 포함되어 있습니다.";
                    break;

                case ErrorList.UnknownVariable:
                    flag = true;
                    TBPtnPreview.Text = "알 수 없는 변수가 사용되었습니다.";
                    break;

                case ErrorList.BlankString:
                    flag = true;
                    TBPtnPreview.Text = "빈 이름은 사용할 수 없습니다.";
                    break;

                case ErrorList.NoError:
                    flag = false;
                    TBPtnPreview.Text = Ptn;
                    break;
            }

            if (!flag) { TBPtnPreview.ToolTip = (new ToolTip().Content = Ptn); }
            else { TBPtnPreview.ToolTip = null; }

            switch (LocationConverter.ConvertAll(TBSaveLoc.Text, out Loc))
            {
                case ErrorList.CannotAccessString:
                    TBLocPreview.Text = "파일명에 사용하지 못하는 문자가 포함되어 있습니다.";
                    TBLocPreview.ToolTip = null;
                    break;

                case ErrorList.UnknownVariable:
                    TBLocPreview.ToolTip = null;
                    break;
                case ErrorList.NoError:
                    TBLocPreview.Text = Loc;
                    TBLocPreview.ToolTip = (new ToolTip().Content = Loc);
                    break;
            }
        }

        private void TBSaveLoc_TextChanged(object sender, TextChangedEventArgs e)
        {
            //switch(LocationConverter.Convert)
        }
    }
}
