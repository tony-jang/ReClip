using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WPFExtension;

namespace ReClip.Control
{
    abstract class ClipItem : ListViewItem
    {
        public abstract void ShowComplete();

        public long Id { get; set; }
    }
}
