using System;

namespace ReClip.Setting
{
    public class ItemCountAttribute : Attribute
    {
        public ItemCountAttribute(int count)
        {
            this.Count = count;
        }
        public int Count { get; set; }
    }
}