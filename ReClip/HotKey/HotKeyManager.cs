using Gma.System.MouseKeyHook;
using System;
using System.Linq;
using System.Collections.Generic;
using f = System.Windows.Forms;

namespace ReClip.HotKey
{
    public static class HotKeyManager
    {
        public static event EventHandler<HotKeyEventArgs> KeyDown;
        public static event EventHandler<HotKeyEventArgs> KeyUp;

        public static event EventHandler<HotKeyEventArgs> PreviewKeyDown;
        public static event EventHandler<HotKeyEventArgs> PreviewKeyUp;

        static List<HotKeyData> items;

        static IKeyboardMouseEvents hook;

        static HotKeyManager()
        {
            items = new List<HotKeyData>();

            hook = Hook.GlobalEvents();
            hook.KeyDown += GlobalHook_KeyDown;
            hook.KeyUp += GlobalHook_KeyUp;
        }

        public static bool AddHotKey(HotKeyData data)
        {
            if (HotKeyManager.Contains(data))
                return false;

            items.Add(data);

            return true;
        }

        public static void AddHotKey(params HotKeyData[] datas)
        {
            foreach (HotKeyData data in datas)
                AddHotKey(data);
        }

        public static bool RemoveHotKey(HotKeyData data)
        {
            if (HotKeyManager.Contains(data))
                return false;

            items.Remove(data);

            return true;
        }
        
        public static bool Contains(HotKeyData data)
        {
            return ContainsFromName(data.Name);
        }

        public static bool ContainsFromName(string name)
        {
            return items.Count(hk => hk.Name == name) > 0;
        }

        public static HotKeyData GetHotKey(string name)
        {
            if (!ContainsFromName(name))
                return null;

            return items.First(hk => hk.Name == name);
        }

        public static IEnumerable<HotKeyData> GetHotKeys()
        {
            return items;
        }

        private static void GlobalHook_KeyDown(object sender, f.KeyEventArgs e)
        {
            OnPreviewKeyDown(new HotKeyEventArgs(e));
        }

        private static void GlobalHook_KeyUp(object sender, f.KeyEventArgs e)
        {
            OnPreviewKeyUp(new HotKeyEventArgs(e));
        }

        private static void OnPreviewKeyDown(HotKeyEventArgs e)
        {
            HotKeyManager.PreviewKeyDown?.Invoke(typeof(HotKeyManager), e);

            if (e.Handled)
                return;

            OnKeyDown(e);
        }

        private static void OnPreviewKeyUp(HotKeyEventArgs e)
        {
            HotKeyManager.PreviewKeyUp?.Invoke(typeof(HotKeyManager), e);

            if (e.Handled)
                return;

            OnKeyUp(e);
        }

        private static void OnKeyDown(HotKeyEventArgs e)
        {
            Execute(HotKeyTrigger.Down, e);

            HotKeyManager.KeyDown?.Invoke(typeof(HotKeyManager), e);
        }

        private static void OnKeyUp(HotKeyEventArgs e)
        {
            Execute(HotKeyTrigger.Up, e);

            HotKeyManager.KeyUp?.Invoke(typeof(HotKeyManager), e);
        }

        private static void Execute(HotKeyTrigger trigger, HotKeyEventArgs e)
        {
            IEnumerable<HotKeyData> hks = GetExecutableHotKeys(trigger, e);
            IEnumerable<HotKeyData> priorityHks = hks.OrderBy(hk => hk.Priority);

            foreach (HotKeyData hk in priorityHks)
            {
                hk.Execute();

                if (hk.Prevent)
                {
                    e.Cancel = true;
                    hk.Reset();
                    return;
                }
            }
        }

        private static IEnumerable<HotKeyData> GetExecutableHotKeys(HotKeyTrigger trigger, HotKeyEventArgs e)
        {
            foreach (HotKeyData hk in items.Where(item => item.Trigger == trigger))
            {
                if (hk.Control ^ e.Control)
                    continue;

                if (hk.Alt ^ e.Alt)
                    continue;

                if (hk.Shift ^ e.Shift)
                    continue;

                if (hk.Key != e.KeyCode)
                    continue;

                yield return hk;
            }
        }
    }
}