using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WSEX = ReClip.Interop.NativeMethods.WS_EX;
using WS = ReClip.Interop.NativeMethods.WS;
using GWL = ReClip.Interop.NativeMethods.GWL;
using System.Windows;
using ReClip.Interop;
using System.Windows.Forms;
using System.Windows.Interop;

namespace ReClip.Windows
{
    public class LayeredWindow : Window
    {
        const byte AC_SRC_OVER = 0x00;
        const byte AC_SRC_ALPHA = 0x01;
        const int ULW_ALPHA = 0x02;

        public LayeredWindow()
        {
            this.ShowInTaskbar = false;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            var helper = new WindowInteropHelper(this);
            var hwndSource = HwndSource.FromHwnd(helper.Handle);

            IntPtr Handle = hwndSource.Handle;

            var ws = (WS)UnsafeNativeMethods.GetWindowLong(Handle, (int)GWL.STYLE);
            var wsex = (WSEX)UnsafeNativeMethods.GetWindowLong(Handle, (int)GWL.EXSTYLE);

            

            ws = WS.VISIBLE | WS.OVERLAPPED | WS.POPUP;
            // | WSEX.NOACTIVATE
            wsex = WSEX.APPWINDOW | WSEX.LAYERED | WSEX.TOPMOST | WSEX.NOACTIVATE;

            UnsafeNativeMethods.SetWindowLong(Handle, (int)GWL.STYLE, (int)ws);
            UnsafeNativeMethods.SetWindowLong(Handle, (int)GWL.EXSTYLE, (int)wsex);
        }
    }
}
