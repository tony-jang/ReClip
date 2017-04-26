using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using ReClip.Util;

namespace ReClip.Database
{
    public class BitmapCacheItem
    {
        public int Id { get; set; }

        public uint CRC32 { get; set; }

        public byte[] Binary { get; set; }

        public BitmapCacheItem()
        {
        }

        public BitmapCacheItem(Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);

                this.Binary = ms.ToArray();
                this.CRC32 = this.Binary.GetCRC32();
            }
        }
    }
}
