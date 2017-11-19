using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ReClip.Setting
{

    [Serializable]
    public class EnvironmentSetting
    {
        public static EnvironmentSetting Default = new EnvironmentSetting();

        public EnvironmentSetting()
        {
            InitalizeAll();
        }

        public void InitalizeAll()
        {
            InitalizeBasis();
            InitalizeClip();
        }

        private void InitalizeBasis()
        {
            ClipboardSaveEnable = true;
            StrectchThumbnail = false;
        }

        private void InitalizeClip()
        {
            SaveCount = ItemSaveTypes.Over30Items;
            ExceptImageItem = false;
            ExceptTextItem = false;
            ExceptFileItem = false;
        }

        #region [  기본 설정  ]
        public bool ClipboardSaveEnable { get; set; }
        public bool StrectchThumbnail { get; set; }
        #endregion

        #region [  클립보드 저장  ]
        
        public ItemSaveTypes SaveCount { get; set; }
        public bool ExceptImageItem { get; set; }
        public bool ExceptTextItem { get; set; }
        public bool ExceptFileItem { get; set; }
        public bool SetStartupProgram { get; set; }
        #endregion

        public EnvironmentSetting Clone()
        {
            return new EnvironmentSetting()
            {
                ClipboardSaveEnable = this.ClipboardSaveEnable,
                ExceptFileItem = this.ExceptFileItem,
                ExceptImageItem = this.ExceptImageItem,
                ExceptTextItem = this.ExceptTextItem,
                SaveCount = this.SaveCount,
                StrectchThumbnail = this.StrectchThumbnail,
                SetStartupProgram = this.SetStartupProgram
            };
        }

        public override bool Equals(object obj)
        {
            if (obj is EnvironmentSetting setting)
            {
                try
                {
                    if (this.ClipboardSaveEnable == setting.ClipboardSaveEnable &&
                        this.ExceptFileItem == setting.ExceptFileItem &&
                        this.ExceptImageItem == setting.ExceptImageItem &&
                        this.ExceptTextItem == setting.ExceptTextItem &&
                        this.SaveCount == setting.SaveCount &&
                        this.StrectchThumbnail == setting.StrectchThumbnail && 
                        this.SetStartupProgram == setting.SetStartupProgram)
                    {
                        return true;
                    }
                }
                catch (Exception) { return false; }
            }

            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

    }

    public enum ItemSaveTypes
    {
        None = 0,
        [ItemCount(10)]
        Over10Items = 1,
        [ItemCount(20)]
        Over20Items = 2,
        [ItemCount(30)]
        Over30Items = 3,
        [ItemCount(50)]
        Over50Items = 4,
        [ItemCount(75)]
        Over75Items = 5,
        [ItemCount(100)]
        Over100Items = 6,
        [ItemCount(-1)]
        InitifiteItems = 20,
    }
}
