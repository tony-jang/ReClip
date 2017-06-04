using System;
using System.Windows.Forms;

namespace ReClip.HotKey
{
    public class HotKeyEventArgs : EventArgs
    {
        public bool Shift { get; }

        public bool Alt { get; }

        public bool Control { get; }

        public bool Handled { get; set; }

        public bool Cancel
        {
            get => natvieKeyEvent.Handled;
            set => natvieKeyEvent.Handled = value;
        }

        public Keys KeyCode { get; }

        public int KeyValue { get; }

        public Keys KeyData { get; }

        public Keys Modifiers { get; }
        
        public bool SuppressKeyPress { get; set; }

        private KeyEventArgs natvieKeyEvent;

        public HotKeyEventArgs(KeyEventArgs args)
        {
            natvieKeyEvent = args;

            this.Alt = args.Alt;
            this.Control = args.Control;
            this.KeyCode = args.KeyCode;
            this.KeyValue = args.KeyValue;
            this.KeyData = args.KeyData;
            this.Modifiers = args.Modifiers;
            this.Shift = args.Shift;
            this.SuppressKeyPress = args.SuppressKeyPress;
        }
    }
}
