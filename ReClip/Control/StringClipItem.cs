using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WPFExtension;

namespace ReClip.Control
{
    [TemplatePart(Name = "CompleteImage", Type = typeof(Image))]
    class StringClipItem : ClipItem
    {
        #region [  FileName Property  ]

        private static readonly DependencyPropertyKey FirstTextPropertyKey = DependencyProperty.RegisterReadOnly("FirstText", typeof(string), typeof(StringClipItem),
            new FrameworkPropertyMetadata("", FrameworkPropertyMetadataOptions.None));

        public static readonly DependencyProperty FirstTextProperty = FirstTextPropertyKey.DependencyProperty;

        public string FirstText
        {
            get { return (string)GetValue(FirstTextProperty); }
            protected set { SetValue(FirstTextPropertyKey, value); }
        }
        #endregion



        Image CompImage;
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            CompImage = Template.FindName("CompleteImage", this) as Image;
        }


        public static DependencyProperty TextProperty = DependencyHelper.Register();
        public string Text { get => (string)GetValue(TextProperty);

            set
            {
                SetValue(TextProperty, value);
                string str = value.Replace("\r\n", " ");
                FirstText = str.Substring(0, (str.Length >= 20 ? 20 : str.Length));
            }
        }


        public StringClipItem()
        {
        }
        public StringClipItem(string itm)
        {
            Text = itm;
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
