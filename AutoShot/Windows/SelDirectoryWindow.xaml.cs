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
using AutoShot.UserControls;
using static AutoShot.Globals.Globals;
using System.Windows.Controls.Primitives;
using AutoShot.Windows;
using System.Threading;

namespace AutoShot.Windows
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
            PART_File.MouseDoubleClick += ListView_MouseDoubleClick;
            PART_Drive.SelectionChanged += DriveItmSelected;
            locTB.KeyDown += LocationKeyDown;

            // 자주 사용하는 폴더 위치 초기화
            itm_Desk.Tag = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            itm_Dcu.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            itm_Dwn.Tag = GetDownloadPath();
            if (!(new DirectoryInfo(GetDownloadPath())).Exists) PART_Drive.Items.Remove(itm_Dwn);
            itm_Music.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
            itm_Pic.Tag = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            itm_exeFolder.Tag = Environment.CurrentDirectory;

            // 사용자의 드라이브 자동 추가
            foreach (DriveInfo di in DriveInfo.GetDrives())
            {
                AddImageItem(di.Name, ImageType.Drive, PART_Drive, "DirItmStyle", di.RootDirectory.FullName);
            }

            ChangeLocation("All Drives");
        }
        private bool IsDirMode = true;
        private void LocationKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ChangeLocation(locTB.Text);
            }
        }

        private void DriveItmSelected(object sender, SelectionChangedEventArgs e)
        {
            string loc = ((ImageItem)PART_Drive.SelectedItem)?.Tag.ToString();
            if (locTB.Text == loc || string.IsNullOrEmpty(loc)) return;
            ChangeLocation(loc);
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DependencyObject obj = ((FrameworkElement)e.OriginalSource).TemplatedParent;
            
            while (obj != null && obj != PART_File)
            {
                if (obj.GetType() == typeof(ImageItem))
                {
                    ImageItem itm = (ImageItem)obj;
                    ChangeLocation(itm.Tag.ToString());

                    break;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
        }
        


        private void FileItmSelected(object sender, SelectionChangedEventArgs e)
        {
            if (PART_File.SelectedItem == null || (PART_File.SelectedItem as ImageItem).Content.ToString() == ".. (상위 폴더)")
            {
                TB_FileName.Text = string.Empty;
                TB_FileType.Text = string.Empty;
                Img_File.Source = GetImage(ImageType.None);
                return;
            }
            string Name = ((ImageItem)PART_File.SelectedItem).Content.ToString();
            string FullName = ((ImageItem)PART_File.SelectedItem).Tag.ToString();
            TB_FileName.Text = Name;
            if (Directory.Exists(FullName)) TB_FileType.Text = "파일 폴더";
            if (File.Exists(FullName)) TB_FileType.Text = new FileInfo(FullName).Extension + " 파일";

            Img_File.Source = ((ImageItem)PART_File.SelectedItem).MainImage;
        }



        public void ChangeLocation(string Location)
        {
            string RealLocation;

            switch (Location.ToLower())
            {
                case "alldrives": case "all drives":
                    RealLocation = "All Drives";
                    break;
                case "desktop":
                    RealLocation = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
                    break;
                case "documents":
                    RealLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                    break;
                case "downloads":
                    RealLocation = GetDownloadPath();
                    break;
                case "music": case "mymusic": case "my music":
                    RealLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic);
                    break;
                case "picture": case "mypicture": case "my picture":
                    RealLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                    break;
                default:
                    RealLocation = Location;
                    break;
            }
            if (File.Exists(RealLocation)) { MsgBox("파일은 열 수 없습니다!","파일 경로 선택 불가"); return; }
            if (!Directory.Exists(RealLocation) && RealLocation != "All Drives") { MsgBox("존재하지 않는 경로입니다!", "경로 오류!", Globals.MessageBoxStyle.OK); locTB.Text = LastLocation; return; }
            PART_File.Items.Clear();

            if (RealLocation == "All Drives")
                foreach (DriveInfo di in DriveInfo.GetDrives())
                    AddImageItem(di.Name, ImageType.Folder, PART_File, "FileItmStyle", di.RootDirectory.FullName);
            else
            {
                string UpperLoc;
                DirectoryInfo dirinfo = new DirectoryInfo(RealLocation).Parent;
                if (dirinfo == null) UpperLoc = "All Drives"; else UpperLoc = dirinfo.FullName;
                
                AddImageItem(".. (상위 폴더)", ImageType.None, PART_File, "FileItmStyle", UpperLoc);
                foreach (DirectoryInfo di in new DirectoryInfo(RealLocation).GetDirectories())
                {
                    if (di.Attributes.HasFlag(FileAttributes.Hidden)) continue;
                    AddImageItem(di.Name, ImageType.Folder, PART_File, "FileItmStyle", di.FullName);
                }

                if (!IsDirMode){
                    foreach (FileInfo fi in new DirectoryInfo(RealLocation).GetFiles())
                    {
                        if (FilterExtensions != null && (fi.Attributes.HasFlag(FileAttributes.Hidden) || !FilterExtensions.Contains(fi.Extension))) continue;
                        string exs = fi.Extension;
                        ImageType it = ImageType.Document;
                        ImageSource img = null;
                        switch (exs)
                        {
                            case ".jpg": case ".png": case ".jpeg": case ".gif": case ".bmp": it = ImageType.Picture;
                                break;
                            case ".mp3": case ".wav": case ".ogg": case ".m4a": case ".flac": it = ImageType.Music;
                                break;
                            case ".xaml": case ".cs": case ".vb": case ".txt": case ".log": case ".xml": it = ImageType.Text;
                                break;
                            case ".xls": case ".xlsx": case ".pptx": case ".ppsx": case ".docx": it = ImageType.Presentation;
                                break;
                            case ".zip": case ".7z": case ".egg": case ".cab": it = ImageType.Zip;
                                break;
                            case ".mp4": case ".wmv": case ".avi": case ".flv": it = ImageType.Video;
                                break;
                            case ".lnk": case ".appref-ms": it = ImageType.Link;
                                break;                              
                            
                            default:
                                it = ImageType.Custom;
                                img = GetIcon(fi.FullName);
                                break;
                        }

                        AddImageItem(fi.Name, it, PART_File, "FileItmStyle", fi.FullName, img);
                    }
                }
            }
            
            locTB.Text = RealLocation;
            bool Flag = false;
            foreach (object itm in PART_Drive.Items)
                if (itm.GetType() == typeof(ImageItem))
                    if (((ImageItem)itm).Tag.ToString() == RealLocation) { PART_Drive.SelectedItem = itm; Flag = true; }

            if (!Flag) PART_Drive.SelectedIndex = -1;

            LastLocation = RealLocation;
            searchTB.Text = string.Empty;

            AllItems = CopyArray(PART_File.Items).Select(x=>(ImageItem)x).ToArray();
        }




        DirectoryInfo ReturnDir;
        FileInfo ReturnFile;
        private string LastLocation = "All Drives";
        private ImageItem[] AllItems = new ImageItem[] { };
        private string[] FilterExtensions = null;

        public DirectoryInfo ShowDialogAsDirectory()
        {
            IsDirMode = true;
            this.Title = "Auto Capturer 폴더 선택";
            base.ShowDialog();

            return ReturnDir;
        }

        public FileInfo ShowDialogAsFile(string[] Extensions = null)
        {
            if (Extensions != null) { FilterExtensions = Extensions; this.Title = $"Auto Capturer 파일 선택 ({string.Join(", ", Extensions)})"; }
            else { this.Title = $"Auto Capturer 파일 선택"; }
            
            
            IsDirMode = false;
            base.ShowDialog();

            return ReturnFile;
        }


        #region [  아이템 추가/제거  ]

        string BaseIconLoc = "/AutoCapturer;component/Resources/Icons/SmallIcons/";

        public void AddImageItem(string Content, ImageType imagetype, ListView Subject,string StyleName,string Tag, ImageSource img = null)
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
                case ImageType.Text: ImgFile = "PresentationIcon"; break;
                case ImageType.Zip: ImgFile = "ZipIcon"; break;
                case ImageType.Presentation: ImgFile = "BookIcon"; break;
                case ImageType.Video: ImgFile = "PlayIcon"; break;
                case ImageType.Link: ImgFile = "LinkIcon"; break;
                case ImageType.Music: ImgFile = "MusicIcon";  break;
            }

            ImageItem itm = new ImageItem();

            itm.Style = FindResource(StyleName) as Style;
            itm.Content = Content;
            itm.Tag = Tag;
            if (imagetype != ImageType.None)
                itm.MainImage = new BitmapImage(new Uri($"{BaseIconLoc}{ImgFile}.png", UriKind.Relative));

            if (imagetype == ImageType.Custom) itm.MainImage = img;

            Subject.Items.Add(itm);
        }

        public BitmapImage GetImage(ImageType imgType)
        {
            string ImgFile = "";

            switch (imgType)
            {
                case ImageType.Drive: ImgFile = "FillFolderIcon"; break;
                case ImageType.Desktop: ImgFile = "DeskIcon"; break;
                case ImageType.Document: ImgFile = "DocumentIcon"; break;
                case ImageType.Download: ImgFile = "DownloadIcon"; break;
                case ImageType.Folder: ImgFile = "FolderIcon"; break;
                case ImageType.Picture: ImgFile = "ImageIcon"; break;
                case ImageType.Text: ImgFile = "PresentationIcon"; break;
                case ImageType.Presentation: ImgFile = "BookIcon"; break;
                case ImageType.Zip: ImgFile = "ZipIcon"; break;
                case ImageType.Link: ImgFile = "LinkIcon"; break;
                case ImageType.Video: ImgFile = "PlayIcon"; break;
                case ImageType.Music: ImgFile = "FillFolderIcon"; break;
            }

            return new BitmapImage(new Uri($"{BaseIconLoc}{ImgFile}.png", UriKind.Relative));
        }
        
        public enum ImageType
        {
            None = 0, Drive = 1, Desktop = 2, Document = 3, Download = 4, Folder = 5, Picture = 6, Music = 7, Text = 8, Presentation = 9, Zip = 10, Link = 11, Video = 12, Custom = 13
        }
        #endregion

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            ReturnDir = null;
            this.Close();
        }
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            
            if (PART_File.SelectedItem == null)
            {
                if (!IsDirMode || LastLocation == "All Drives") { MsgBox("선택된 파일/디렉터리가 없습니다!", "디렉터리 없음!"); return; }

                ReturnDir = new DirectoryInfo(LastLocation);
                this.Close();
                return;
            }
            string location = ((ImageItem)PART_File.SelectedItem).Tag.ToString();
            if (IsDirMode)
            {
                if (!Directory.Exists(location)) { MsgBox("디렉토리를 선택하셔야 합니다.", "선택 오류!"); return; }
                ReturnDir = new DirectoryInfo(location);
            }
            else
            {
                if (!File.Exists(location)) { MsgBox("파일을 선택하셔야 합니다.", "선택 오류!"); return; }
                ReturnFile = new FileInfo(location);
            }
            
            this.Close();
        }

        private void searchTB_TextChanged(object sender, TextChangedEventArgs e)
        {
            bool AllAdd = false;
            if (string.IsNullOrEmpty(searchTB.Text)) AllAdd = true;

            PART_File.Items.Clear();
            foreach (ImageItem itm in AllItems)
            {
                if (itm.Content.ToString() == ".. (상위 폴더)" || 
                    itm.Content.ToString().ToLower().Contains(searchTB.Text.ToLower()) ||
                    AllAdd) PART_File.Items.Add(itm);
            }
        }

    }
}
namespace AutoShot.Globals
{
    public partial class Globals
    {
        public static DirectoryInfo ShowSelectDirectoryDialog()
        {
            var wdw = new SelDirectoryWindow();
            
            return wdw.ShowDialogAsDirectory();
        }
        public static FileInfo ShowSelectFileDialog(string[] Filters = null)
        {
            var wdw = new SelDirectoryWindow();

            return wdw.ShowDialogAsFile(Filters);

        }
    }
}