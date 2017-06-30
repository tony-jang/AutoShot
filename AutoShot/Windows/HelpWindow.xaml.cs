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

namespace AutoShot.Windows
{
    /// <summary>
    /// HelpWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class HelpWindow : ChromeWindow
    {
        public HelpWindow()
        {
            InitializeComponent();
            treeView.SelectedItemChanged += TreeView_SelectedItemChanged;
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            frame.Content = ((TreeViewItem)treeView.SelectedItem).Tag;
        }
    }
}
