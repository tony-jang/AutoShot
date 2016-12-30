using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AutoCapturer.UserControls
{
    class PatternItem : RadioButton
    {

        public PatternItem(){
            this.Style = FindResource("PatternItemStyle") as Style;
        }

        public static DependencyProperty SavePatternProperty =
            DependencyProperty.Register(nameof(SavePattern), typeof(string),
                typeof(PatternItem),new PropertyMetadata(string.Empty));

        public string SavePattern
        {
            get { return (string)GetValue(SavePatternProperty); }
            set { SetValue(SavePatternProperty, value); }
        }
    }
}
