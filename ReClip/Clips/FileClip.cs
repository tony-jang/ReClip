using LiteDB;
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
        public FileClip(string[] Data) : this(Data, KeyGenerator.GenerateKey())
        {
        }

        public FileClip(string[] data, long Key) : this()
        {
            this.Data = Data;

            Id = Key;
        }

        public FileClip()
        {
            this.date = DateTime.Now;
        }

     
        public ClipboardFormat Format { get => ClipboardFormat.FileDrop; }
        public string[] Data { get; set; }
        public long Id { get; set; }
        public DateTime date { get; set; }
    }
}
