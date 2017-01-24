using AutoCapturer.Windows;
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

namespace AutoCapturer.Globals
{
    /// <summary>
    /// MessageBoxWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageBoxWindow : ChromeWindow
    {
        
        internal MessageBoxWindow(string Title, string Message, MessageBoxStyle msgboxStyle)
        {
            InitializeComponent();

            frmstyle = msgboxStyle;
            returndata = MessageBoxResult.None;

            this.Title = Title;
            this.messageTB.Text = Message;

            this.Closing += FrmClosing;
            this.KeyDown += KD;

            switch (msgboxStyle)
            {
                case MessageBoxStyle.OKCancel:
                    BtnOK.Content = "확인";
                    BtnCancel.Content = "취소";
                    break;
                case MessageBoxStyle.Yes:
                    BtnOK.Content = "예";
                    BtnCancel.Visibility = Visibility.Hidden;
                    break;
                case MessageBoxStyle.YesNo:
                    BtnOK.Content = "예";
                    BtnCancel.Content = "아니오";
                    break;
                case MessageBoxStyle.OK:
                    BtnOK.Content = "확인";
                    BtnCancel.Visibility = Visibility.Hidden;
                    break;
            }
            
        }

        private void KD(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnClick(BtnOK, null);
            }
        }

        private void FrmClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (returndata == MessageBoxResult.None)
            {
                switch (frmstyle)
                {
                    case MessageBoxStyle.YesNo:
                        returndata = MessageBoxResult.No;
                        break;
                    case MessageBoxStyle.Yes:
                        returndata = MessageBoxResult.Yes;
                        break;
                    case MessageBoxStyle.OKCancel:
                        returndata = MessageBoxResult.Cancel;
                        break;
                    case MessageBoxStyle.OK:
                        returndata = MessageBoxResult.OK;
                        break;
                }
            }
            
        }

        private MessageBoxResult returndata;
        private MessageBoxStyle frmstyle;

        internal new MessageBoxResult ShowDialog()
        {
            base.ShowDialog();

            return returndata;
        }

        private void BtnClick(object sender, RoutedEventArgs e)
        {
            switch (((Button)sender).Content.ToString())
            {
                case "확인":
                    returndata = MessageBoxResult.OK;
                    break;
                case "취소":
                    returndata = MessageBoxResult.Cancel;
                    break;
                case "예":
                    returndata = MessageBoxResult.Yes;
                    break;
                case "아니오":
                    returndata = MessageBoxResult.No;
                    break;
            }
            this.Close();
        }
    }

    static partial class Globals
    {
        public static MessageBoxResult MsgBox(string Message,string Title = "", MessageBoxStyle msgboxstyle = MessageBoxStyle.OK)
        {
            MessageBoxWindow wdw = new MessageBoxWindow(Title, Message, msgboxstyle);

            return wdw.ShowDialog();
        }
    }

    public enum MessageBoxStyle
    {
        YesNo, Yes, OKCancel, OK
    }
}
