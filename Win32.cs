using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.InteropServices;
namespace NamaAlert
{
    public class Win32
    {
        public const uint WS_SYSMENU = 0x80000;
        public const uint WS_MINIMIZEBOX = 0x20000;
        public const uint WS_MAXIMIZEBOX = 0x10000;

        [DllImport("user32")]
        public static extern uint GetWindowLong(IntPtr hWnd, int index);

        [DllImport("user32")]
        public static extern uint SetWindowLong(IntPtr hWnd, int index, uint dwLong);
    }
}
