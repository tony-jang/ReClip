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
        public StringClip(string Data) : this(Data, KeyGenerator.GenerateKey())
        {
        }

        public StringClip(string Data, long Key) : this()
        {
            this.Data = Data;
            Id = Key;
        }

        public StringClip()
        {
            this.date = DateTime.Now;
            _data = "";
        }

        public ClipboardFormat Format { get => ClipboardFormat.Text; }

        private string _data;
        public string Data { get => _data; set => _data = value; }
        public long Id { get; set; }
        public DateTime date { get; set; }
    }
}
