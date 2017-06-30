using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoShot.Utils
{
    public enum ClipboardFormat : byte
    {
        None,
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
