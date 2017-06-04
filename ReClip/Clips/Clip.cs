using LiteDB;
using ReClip.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClip.Clips
{
    public interface Clip
    {
        [BsonId]
        long Id { get; set; }
        ClipboardFormat Format { get; }
    }
}
