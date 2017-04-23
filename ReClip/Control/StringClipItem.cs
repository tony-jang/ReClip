using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPFExtension;

namespace ReClip.Control
{
    class StringClipItem : ClipItem
    {
        public static DependencyProperty TextProperty = DependencyHelper.Register();
        public string Text { get => (string)GetValue(TextProperty); set => SetValue(TextProperty, value); }


        public StringClipItem()
        {

        }
    }
}
