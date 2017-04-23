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
            base.Format = ClipboardFormat.Text;
            this.Data = Data;
        }
        public StringClip()
        {
            base.Format = ClipboardFormat.Text;
            _data = "";
        }
        private string _data;
        public string Data { get => _data; set => _data = value; }
    }
}
