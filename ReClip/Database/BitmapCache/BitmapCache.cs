using LiteDB;
using ReClip.Util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ReClip.Database
{

    public class BitmapCache : IDisposable
    {
        static BitmapCache db;

        LiteDatabase liteDb;
        LiteCollection<BitmapCacheItem> collection;

        private BitmapCache()
        {
            string path = $"{Environment.UserName}.cache.db";
            //if (File.Exists(path)) File.Delete(path);
            liteDb = new LiteDatabase(path);
            collection = liteDb.GetCollection<BitmapCacheItem>("bitmaps");
        }

        public void Dispose()
        {
            liteDb?.Dispose();
            liteDb = null;
        }

        private BitmapCacheItem GetCacheItem(uint crc32)
        {
            return collection.FindOne(item => item.CRC32 == crc32);
        }

        private void Add(Bitmap bitmap)
        {
            if (Contains(bitmap))
                return;

            collection.Insert(new BitmapCacheItem(bitmap));
        }

        private void Remove(Bitmap bitmap)
        {
            Remove(bitmap.GetCRC32());
        }

        private void Remove(uint crc32)
        {
            if (!Contains(crc32))
                return;

            collection.Delete(item => item.CRC32 == crc32);
        }

        public bool Contains(Bitmap bitmap)
        {
            return Contains(bitmap.GetCRC32());
        }

        public bool Contains(uint crc32)
        {
            return collection
                .Find(item => item.CRC32 == crc32)
                .Count() > 0;
        }

        public static void Open()
        {
            if (db == null)
                db = new BitmapCache();
        }

        public static void Close()
        {
            db?.Dispose();
            db = null;
        }
        
        public static void AddBitmap(Bitmap bitmap)
        {
            db.Add(bitmap);
        }

        public static void RemoveBitmap(Bitmap bitmap)
        {
            db.Remove(bitmap);
        }

        public static bool ContainsBitmap(Bitmap bitmap)
        {
            return db.Contains(bitmap);
        }

        public static Bitmap GetBitmapFromCRC32(uint crc32)
        {
            var item = db.GetCacheItem(crc32);

            if (item == null)
                return null;
            
            return new Bitmap(new MemoryStream(item.Binary));
        }

        public static IEnumerable<BitmapCacheItem> GetCachedBitmaps()
        {
            return db.collection
                .FindAll();
        }
    }
}
