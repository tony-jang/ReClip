using LiteDB;
using ReClip.Clips;
using ReClip.Setting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReClip.Database
{
    public class SettingDB
    {
        LiteCollection<EnvironmentSetting> coll;
        LiteDatabase db;
        
        public SettingDB()
        {
            db = new LiteDatabase(Path.Combine(Path.GetTempPath() ,"ReClipDB.db"));
            coll = db.GetCollection<EnvironmentSetting>("setting");
        }
        

        public EnvironmentSetting GetSetting()
        {
            if (coll.Count() == 0)
            {
                coll.Insert(new EnvironmentSetting());
            }
            return coll.FindAll().First();
        }
        public void SetSetting(EnvironmentSetting Setting)
        {
            db.DropCollection("setting");
            coll = db.GetCollection<EnvironmentSetting>("setting");
            coll.Insert(Setting);
        }
    }
}
