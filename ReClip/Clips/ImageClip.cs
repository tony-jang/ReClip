﻿using ReClip.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ReClip.Clips
{
    [Serializable]
    class ImageClip : Clip
    {
        public ImageClip(ImageSource Image)
        {
            this.Image = Image;
            Time = DateTime.Now;
        }
        public ImageSource Image { get; set; }
        public ClipboardFormat Format { get => ClipboardFormat.FileDrop; }
        public DateTime Time { get; set; }
    }
}