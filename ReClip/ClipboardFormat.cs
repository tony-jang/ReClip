using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClip
{
    public enum ClipboardFormat : byte
    {
        Text,
        UnicodeText,
        Dib,

        Bitmap,
        EnhancedMetafile,
        MetafilePict,
        SymbolicLink,
        Dif,
        Tiff,
        OemText,
        Palette,

        PenData,
        Riff,
        WaveAudio,
        FileDrop,
        Locale,
        Html,
        Rtf,

        CommaSeparatedValue,
        StringFormat,
        Serializable,
    }
}
