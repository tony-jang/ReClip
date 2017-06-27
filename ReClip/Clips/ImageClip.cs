using ReClip.Util;
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
        public ImageClip(uint CRC32) : this(CRC32, KeyGenerator.GenerateKey())
        {
        }

        public ImageClip(uint CRC32, long Key) : this()
        {
            this.CRC32 = CRC32;
            Id = Key;
        }

        public ImageClip()
        {
            this.date = DateTime.Now;
        }

        public uint CRC32 { get; set; }
        public ClipboardFormat Format { get => ClipboardFormat.FileDrop; }
        public long Id { get; set; }
        public DateTime date { get; set; }
    }
}