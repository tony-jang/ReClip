using ReClip.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClip.Clips
{
    [Serializable]
    class StringClip : Clip
    {
        public StringClip(string Data)
        {
            this.Data = Data;
            Time = DateTime.Now;
        }
        public StringClip()
        {
            _data = "";
        }

        public ClipboardFormat Format { get => ClipboardFormat.Text; }

        private string _data;
        public string Data { get => _data; set => _data = value; }
        public DateTime Time { get; set; }
    }
}
