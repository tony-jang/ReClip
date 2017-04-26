using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ReClip.Util
{
    internal static class IconUtilities
    {
        private static Dictionary<uint, BitmapImage> bitmapCache =
            new Dictionary<uint, BitmapImage>();

        public static ImageSource ToImageSource(this Icon icon)
        {
            return ToBitmapImage(icon.ToBitmap());
        }

        public static Bitmap ToBitmap(this BitmapImage bitmapImage)
        {
            // BitmapImage bitmapImage = new BitmapImage(new Uri("../Images/test.png", UriKind.Relative));

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapImage));
                enc.Save(outStream);
                Bitmap bitmap = new Bitmap(outStream);

                return new Bitmap(bitmap);
            }
        }

        public static uint GetCRC32(this Bitmap bitmap)
        {
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);

                return Crc32C.Crc32CAlgorithm.Compute(ms.ToArray());
            }   
        }

        public static uint GetCRC32(this byte[] binary)
        {
            return Crc32C.Crc32CAlgorithm.Compute(binary);
        }

        public static BitmapSource ToBitmapImage(this Bitmap bitmap)
        {

            return (BitmapSource)Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
                                                                      BitmapSizeOptions.FromWidthAndHeight(bitmap.Width, bitmap.Height));


            var bitmapImage = new BitmapImage();
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                memory.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        public static ImageSource ToThumbnail(this Bitmap bitmap)
        {
            BitmapImage image;
            uint crc32 = bitmap.GetCRC32();

            if (!bitmapCache.TryGetValue(crc32, out image))
            {
                var thumbnail = bitmap.GetThumbnailImage(120, 100, null, IntPtr.Zero);
                var streamSource = new MemoryStream();

                thumbnail.Save(streamSource, ImageFormat.Png);
                thumbnail.Dispose();

                image = new BitmapImage();

                image.BeginInit();
                image.StreamSource = streamSource;
                image.CacheOption = BitmapCacheOption.OnLoad;
                image.EndInit();

                bitmapCache[crc32] = image;
            }
            
            return image;
        }
    }
}
