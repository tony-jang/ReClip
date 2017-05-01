using ReClip.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ReClip.Control
{
    [TemplatePart(Name = "TBFileCount", Type = typeof(OutlinedTextBlock))]
    [TemplatePart(Name = "CompleteImage", Type = typeof(Image))]
    class FileClipItem : ClipItem
    {
        #region [  FileName Property  ]

        private static readonly DependencyPropertyKey FileNamePropertyKey = DependencyProperty.RegisterReadOnly("FileName", typeof(string), typeof(FileClipItem),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty FileNameProperty = FileNamePropertyKey.DependencyProperty;

        public string FileName
        {
            get { return (string)GetValue(FileNameProperty); }
            protected set { SetValue(FileNamePropertyKey, value); }
        }

        #endregion

        #region [  FileIcon Property  ]

        private static readonly DependencyPropertyKey FileIconPropertyKey = DependencyProperty.RegisterReadOnly("FileIcon", typeof(ImageSource), typeof(FileClipItem),
    new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty FileIconProperty = FileIconPropertyKey.DependencyProperty;

        public ImageSource FileIcon
        {
            get { return (ImageSource)GetValue(FileIconProperty); }
            protected set { SetValue(FileIconPropertyKey, value); }
        }

        #endregion

        #region [  FilesCount Property  ]

        private static readonly DependencyPropertyKey ExFilesCountPropertyKey = DependencyProperty.RegisterReadOnly("ExFilesCount", typeof(string), typeof(FileClipItem),
            new FrameworkPropertyMetadata("0", FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty ExFilesCountProperty = ExFilesCountPropertyKey.DependencyProperty;

        public string ExFilesCount
        {
            get { return (string)GetValue(ExFilesCountProperty); }
            protected set { SetValue(ExFilesCountPropertyKey, value); }
        }

        #endregion



        public FileClipItem(string[] FilePaths) : base()
        {
            this.FilePaths = FilePaths;
            if (Directory.Exists(FilePaths.First()))
            {
                var itm = new BitmapImage(new Uri("pack://application:,,,/ReClip;component/Image/FolderIcon.jpg"));
                FileIcon = itm;
            }
            else
            {
                var icon = System.Drawing.Icon.ExtractAssociatedIcon(FilePaths.First()).ToImageSource();
                FileIcon = icon;
            }
            
        }
        public FileClipItem() : base()
        {
            FilePaths = new string[] { };
        }


        OutlinedTextBlock TBFileCount;
        Image CompImage;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TBFileCount = Template.FindName("TBFileCount",this) as OutlinedTextBlock;
            CompImage = Template.FindName("CompleteImage", this) as Image;

            if (LastState.Item1) {
                TBFileCount.Visibility = LastState.Item2;
                LastState.Item1 = false;
            }
        }

        public override void ShowComplete()
        {
            Thread thr = new Thread(() =>
            {
                Dispatcher.Invoke(() => { CompImage.Visibility = Visibility.Visible; });
                Thread.Sleep(1000);
                Dispatcher.Invoke(() => { CompImage.Visibility = Visibility.Hidden; });
            });
            thr.Start();
        }


        (bool, Visibility) LastState = (false, Visibility.Collapsed);
        string[] _FilePaths = new string[] { };
        public string[] FilePaths
        {
            get=> _FilePaths;
            set
            {
                _FilePaths = value;
                ExFilesCount = $"+{(value.Count() - 1).ToString()}";

                bool Flag = false;
                if (value.Length > 0)
                {
                    FileInfo fi = new FileInfo(value.First());

                    if (fi.Exists)
                    {
                        Flag = true;
                        FileName = fi.Name;
                        var img = System.Drawing.Icon.ExtractAssociatedIcon(fi.FullName).ToImageSource();
                        FileIcon = img;
                    }
                    else if (Directory.Exists(value.First()))
                    {
                        FileName = new DirectoryInfo(value.First()).Name;
                        Flag = true;
                    }
                    
                    if (ExFilesCount == "+0")
                    {
                        try { TBFileCount.Visibility = Visibility.Hidden; }
                        catch { LastState = (true, Visibility.Hidden); }
                    }
                    else
                    {
                        try { TBFileCount.Visibility = Visibility.Visible; }
                        catch (Exception) { LastState = (true, Visibility.Visible); }
                    }
                }

                if (!Flag)
                {
                    FileName = "";
                    FileIcon = null;
                }
            }
        }
    }
}
