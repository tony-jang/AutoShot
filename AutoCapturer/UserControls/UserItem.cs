using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AutoCapturer.UserControls
{
    class UserItem : ListViewItem
    {

        public UserItem(){
            this.Style = FindResource("UserItemStyle") as Style;
        }

        public static DependencyProperty UserImageProperty =
            DependencyProperty.Register(nameof(UserImage), typeof(ImageSource),
            typeof(UserItem), new PropertyMetadata(new BitmapImage()));

        public ImageSource UserImage
        {
            get { return (ImageSource)GetValue(UserImageProperty); }
            set { SetValue(UserImageProperty, value); }
        }
    }
}
