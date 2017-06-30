using AutoShot.Setting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AutoShot.UserControls
{
    class PatternItem : ListViewItem
    {
        public PatternItem(string SaveLoc, string SaveName, SavePattern ptn)
        {
            this.Style = FindResource("PatternItemStyle") as Style;
            SaveLocation = SaveLoc;
            Content = SaveName;
            pattern = ptn;
        }
        public PatternItem(){
            this.Style = FindResource("PatternItemStyle") as Style;
        }

        public static DependencyProperty SaveLocationProperty =
            DependencyProperty.Register(nameof(SaveLocation), typeof(string),
                typeof(PatternItem), new PropertyMetadata(string.Empty));

        public static DependencyProperty IsDefaultPatternProperty =
            DependencyProperty.Register(nameof(IsDefaultPattern), typeof(bool),
                typeof(PatternItem), new PropertyMetadata(false));

        public string SaveLocation
        {
            get { return (string)GetValue(SaveLocationProperty); }
            set { SetValue(SaveLocationProperty, value); }
        }

        public bool IsDefaultPattern
        {
            get { return (bool)GetValue(IsDefaultPatternProperty); }
            set { SetValue(IsDefaultPatternProperty, value); }
        }

        public SavePattern pattern;
    }
}
