using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFExtension;

namespace ReClip.Control
{
    [TemplatePart(Name = "CompleteImage", Type = typeof(Image))]
    class ImageClipItem : ClipItem
    {
        public static DependencyProperty SourceProperty = DependencyHelper.Register();

        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        public uint CRC32 { get; }

        public ImageClipItem()
        {

        }
        
        public ImageClipItem(ImageSource image, uint crc32)
        {
            this.Source = image;
            this.CRC32 = crc32;
        }

        Image CompImage;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CompImage = Template.FindName("CompleteImage", this) as Image;
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
    }
}
