using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoShot.UserControls
{
    class ImageItem : ListViewItem
    {

        public ImageItem(){
            this.Style = FindResource("UserItemStyle") as Style;
        }

        public static DependencyProperty MainImageProperty =
            DependencyProperty.Register(nameof(MainImage), typeof(ImageSource),
            typeof(ImageItem), new PropertyMetadata(new BitmapImage()));

        public ImageSource MainImage
        {
            get { return (ImageSource)GetValue(MainImageProperty); }
            set { SetValue(MainImageProperty, value); }
        }
    }
}
