using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AutoCapturer.UserControls
{
    class StoargySpaceToolTip : ToolTip
    {
        public StoargySpaceToolTip()
        {
            this.Style = FindResource("StoargySpaceToolTipStyle") as Style;
        }
        

        public static DependencyProperty RemainPictureCountProperty = DependencyProperty.Register(nameof(RemainPictureCount),
            typeof(string), typeof(StoargySpaceToolTip), new PropertyMetadata(string.Empty));

        public string RemainPictureCount
        {
            get { return (string)GetValue(RemainPictureCountProperty); }
            set { SetValue(RemainPictureCountProperty, value); }
        }

        public static DependencyProperty CalcFileSizeProperty = DependencyProperty.Register(nameof(CalcFileSize),
                    typeof(string), typeof(StoargySpaceToolTip), new PropertyMetadata(string.Empty));

        public string CalcFileSize
        {
            get { return (string)GetValue(CalcFileSizeProperty); }
            set { SetValue(CalcFileSizeProperty, value); }
        }


    }
}
