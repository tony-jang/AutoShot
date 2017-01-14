using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
using System.Windows.Controls.Primitives;

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

            // 이벤트 핸들러 연결
            PART_File.SelectionChanged += FileItmSelected;
            PART_File.MouseDoubleClick += FileItmDoubleClick;
            // 자주 사용하는 폴더 위치 초기화
            itm_Desk.Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            itm_Dcu.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            itm_Dwn.Tag = GetDownloadPath();
            if (!(new DirectoryInfo(GetDownloadPath())).Exists) PART_Drive.Items.Remove(itm_Dwn);
            itm_Music.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            itm_Pic.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            // 사용자의 드라이브 자동 추가
            foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives())
            {
                AddImageItem(di.Name, ImageType.Drive, PART_Drive, "DirItmStyle",string.Empty);
                AddImageItem(di.Name, ImageType.Folder, PART_File, "FileItmStyle", di.RootDirectory.FullName);
            }
        }

        private void FileItmDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var item = PART_File.SelectedItem as Track;
            if (item != null)
            {
                MessageBox.Show(item.ToString() + " Double Click handled!");
            }
        }

        private void FileItmSelected(object sender, SelectionChangedEventArgs e)
        {
            TB_FileName.Text = ((ImageItem)PART_File.SelectedItem).Content.ToString();
            TB_FileType.Text = "파일 폴더";
            Img_File.Source = ((ImageItem)PART_File.SelectedItem).MainImage;
        }

        public void SelectItemByLocation(string location)
        {
            bool flag = false;

            foreach (object itm in PART_Drive.Items)
            {
                ImageItem imageitm = null;
                if (itm.GetType() == typeof(ImageItem)) imageitm = (ImageItem)itm;
                else continue;

                if ((string)imageitm.Tag == location) { flag = true; PART_Drive.SelectedItem = itm;}
            }
            if (!flag) PART_Drive.SelectedItem = null;
        }

        #region [  아이템 추가/제거  ]

        string BaseIconLoc = "/AutoCapturer;component/Resources/Icons/SmallIcons/";

        public void AddImageItem(string Content, ImageType imagetype, ListView Subject,string StyleName,string Tag)
        {
            string ImgFile = "";

            switch (imagetype)
            {
                case ImageType.Drive: ImgFile = "FillFolderIcon";  break;
                case ImageType.Desktop: ImgFile = "DeskIcon";      break;
                case ImageType.Document: ImgFile = "DocumentIcon"; break;
                case ImageType.Download: ImgFile = "DownloadIcon"; break;
                case ImageType.Folder: ImgFile = "FolderIcon";     break;
                case ImageType.Picture: ImgFile = "ImageIcon";     break;
                case ImageType.Music: ImgFile = "FillFolderIcon";  break;
            }

            ImageItem itm = new ImageItem();

            itm.Style = FindResource(StyleName) as Style;
            itm.Content = Content;
            itm.Tag = Content;
            if (imagetype != ImageType.None)
                itm.MainImage = new BitmapImage(new Uri($"{BaseIconLoc}{ImgFile}.png", UriKind.Relative));

            Subject.Items.Add(itm);
        }
        public enum ImageType
        {
            None = 0, Drive = 1, Desktop = 2, Document = 3, Download = 4, Folder = 5, Picture = 6, Music = 7
        }
        #endregion

    }
}
