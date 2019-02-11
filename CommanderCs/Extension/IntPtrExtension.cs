using Commander.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Commander.Extension
{
    static class IntPtrExtension
    {
        public static WindowStyles SetWindowStyle(this IntPtr hwnd, WindowStyles style)
            => (WindowStyles)Api.SetWindowLong(hwnd, WindowLongValue.Style, (uint)style);

        public static void SetTop(this IntPtr hwnd)
            => Api.SetWindowPos(hwnd, IntPtr.Zero, 0, 0, 0, 0, SetWindowPosFlags.IgnoreMove | SetWindowPosFlags.IgnoreResize);
    }
}
