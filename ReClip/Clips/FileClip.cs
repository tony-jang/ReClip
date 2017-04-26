using ReClip.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClip.Clips
{
    [Serializable]
    class FileClip : Clip
    {
        public FileClip(string[] Data)
        {
            this.Data = Data;
            Time = DateTime.Now;
        }
        public ClipboardFormat Format { get => ClipboardFormat.FileDrop; }
        public string[] Data { get; set; }
        public DateTime Time { get; set; }
    }
}
