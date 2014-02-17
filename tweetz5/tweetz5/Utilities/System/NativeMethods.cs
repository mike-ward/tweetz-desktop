using System;
using System.Runtime.InteropServices;

namespace tweetz5.Utilities.System
{
    public static class NativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, IntPtr dwExtraInfo);
    }
}