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
using AutoCapturer.UserControls;
using static AutoCapturer.Globals.Globals;

namespace AutoCapturer.Windows
{
    /// <summary>
    /// SelDirectoryWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SelDirectoryWindow : ChromeWindow
    {
        public SelDirectoryWindow()
        {

            InitializeComponent();


            itm_Desk.Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            itm_Dcu.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            itm_Dwn.Tag = GetDownloadPath();
            itm_Music.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            itm_Pic.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);


            foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives())
            {
                AddDriveItem(di.Name, ImageType.Drive);
                AddFolderItems(di.Name, di.RootDirectory.FullName);
            }
        }

        public void AddDriveItem(string Content, ImageType imagetype)
        {
            string FileName = "";

            switch (imagetype)
            {
                case ImageType.Drive:
                    FileName = "FillFolderIcon";
                    break;
                case ImageType.Desktop:
                    FileName = "DeskIcon";
                    break;
                case ImageType.Document:
                    FileName = "DocumentIcon";
                    break;
                case ImageType.Download:
                    FileName = "DownloadIcon";
                    break;
                case ImageType.Folder:
                    FileName = "FolderIcon";
                    break;
                case ImageType.Picture:
                    FileName = "ImageIcon";
                    break;
                case ImageType.Music:
                    FileName = "FillFolderIcon";
                    break;
            }

            ImageItem itm = new ImageItem();

            itm.Style = FindResource("DirItem") as Style;
            itm.Content = Content;
            itm.Tag = Content;
            if (imagetype != ImageType.None)
                itm.MainImage = new BitmapImage(new Uri($"/AutoCapturer;component/Resources/Icons/SmallIcons/{FileName}.png", UriKind.Relative));

            PART_Drive.Items.Add(itm);
        }


        public void AddFolderItems(string Content, string Tag)
        {
            BitmapImage img = new BitmapImage(new Uri($"/AutoCapturer;component/Resources/Icons/SmallIcons/FolderIcon.png", UriKind.Relative));
            ImageItem itm = new ImageItem();

            itm.Style = FindResource("DirItmStyle") as Style;
            itm.Content = Content;
            itm.Tag = Tag;
            itm.MainImage = img;

            PART_File.Items.Add(itm);
        }

        public enum ImageType
        {
            None = 0,
            Drive = 1,
            Desktop = 2,
            Document = 3,
            Download = 4,
            Folder = 5,
            Picture = 6,
            Music = 7
        }

        public string ViewLocation = "AllDrives";

        private void PART_File_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PART_File.SelectedItem == null) return;
            string location = (string)(PART_File.SelectedItem as ImageItem).Tag;

            locTB.Text = location;

            PART_File.Items.Clear();

            if (location == "AllDrives")
            {
                foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives())
                {
                    AddFolderItems(di.Name, di.RootDirectory.FullName);
                }
                PART_Drive.SelectedItem = null;
                return;
            }


            AddItemByLocation(location);
            SelectItemByLocation(location);



        }

        private void locTB_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                string location = locTB.Text;

                locTB.Text = location;

                PART_File.Items.Clear();

                if (location == "AllDrives")
                {
                    foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives())
                    {
                        AddFolderItems(di.Name, di.RootDirectory.FullName);
                    }
                    PART_Drive.SelectedItem = null;
                    return;
                }
                
                if (string.IsNullOrEmpty(location) || !(new System.IO.DirectoryInfo(location)).Exists)
                {
                    MessageBox.Show("사용할 수 없는 경로입니다!");
                    return;
                }

                AddItemByLocation(location);
                SelectItemByLocation(location);
            }
        }

        private void PART_Drive_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ImageItem itm = (ImageItem)PART_Drive.SelectedItem;

            if (itm == null) return;

            string location = (string)itm.Tag;

            locTB.Text = location;

            PART_File.Items.Clear();

            AddItemByLocation(location);

        }

        public void AddItemByLocation(string location)
        {
            if (string.IsNullOrEmpty(location)) return;

            if ((new System.IO.DirectoryInfo(location)).Parent != null)
            {
                ViewLocation = "AllDrives";
                AddFolderItems(".. (상위 폴더)", (new System.IO.DirectoryInfo(location)).Parent.FullName);
            }
            else
            {
                AddFolderItems(".. (상위 폴더)", "AllDrives");
            }
            try
            {
                ViewLocation = location;
                foreach (System.IO.DirectoryInfo di in new System.IO.DirectoryInfo(location).GetDirectories())
                {
                    if (!di.Attributes.HasFlag(System.IO.FileAttributes.Hidden))
                        AddFolderItems(di.Name, di.FullName);
                }
            }
            catch
            { }
        }
        public void SelectItemByLocation(string location)
        {
            bool flag = false;

            foreach (object itm in PART_Drive.Items)
            {
                ImageItem imageitm = null;
                if (itm.GetType() == typeof(ImageItem)) imageitm = (ImageItem)itm;
                else continue;
                if ((string)imageitm.Tag == location)
                {
                    flag = true;
                    PART_Drive.SelectedItem = itm;
                }
            }
            if (!flag) PART_Drive.SelectedItem = null;
        }
    }
}
