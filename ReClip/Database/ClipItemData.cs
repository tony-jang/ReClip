using LiteDB;
using ReClip.Clips;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ReClip.Database
{
    class ClipItemData
    {
        LiteCollection<Clip> coll;
        LiteDatabase db;
        public ClipItemData()
        {
            db = new LiteDatabase(Path.Combine(Path.GetTempPath(), "ReClipDB.db"));
            coll = db.GetCollection<Clip>("Items");
        }

        public void Add(Clip clip)
        {
            coll.Insert(clip);
        }
        public void AddRange(List<Clip> clips)
        {
            foreach (Clip c in clips)
            {
                coll.Insert(c);
            }
        }

        public void Clear()
        {
            db.DropCollection("Items");
            coll = db.GetCollection<Clip>("Items");
        }

        public void SetTo(List<Clip> clips)
        {
            Clear();
            AddRange(clips);
        }
        public List<Clip> GetAllItem()
        {
            return coll.FindAll().ToList();
        }
        public int Count()
        {
            return coll.Count();
        }

        public bool Remove(Clip clip)
        {
            return coll.Delete(clip.Id);
        }
        public bool Remove(long Id)
        {
            return coll.Delete(Id);
        }

        public bool Contains(Clip clip)
        {
            try
            {
                return coll.FindById((long)clip.Id) != null;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
