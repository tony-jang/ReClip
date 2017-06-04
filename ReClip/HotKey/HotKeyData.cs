using System;
using System.Windows.Forms;

namespace ReClip.HotKey
{
    public enum HotKeyTrigger
    {
        Down,
        Up
    }

    public enum HotKeyPriority
    {
        Highest = 0,
        Normal = 1,
        Lowest = 2
    }

    public class HotKeyData
    {
        public string Name { get; set; }

        public Keys Key { get; set; }

        public bool Shift { get; set; }

        public bool Alt { get; set; }

        public bool Control { get; set; }

        public Action<HotKeyData> Action { get; set; }

        public bool Prevent { get; set; } = false;

        public HotKeyTrigger Trigger { get; set; }

        public HotKeyPriority Priority { get; set; }

        public void Execute()
        {
            this.Action?.Invoke(this);
        }

        public void Reset()
        {
            this.Prevent = false;
        }
    }
}
