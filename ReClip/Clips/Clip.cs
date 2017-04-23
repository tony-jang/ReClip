using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClip.Clips
{
    [Serializable]
    public abstract class Clip
    {
        public ClipboardFormat Format { get; internal set; }
    }
}
