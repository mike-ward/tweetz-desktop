using System.Windows;
using System.Windows.Interop;
using Display = System.Windows.Forms.Screen;

namespace tweetz5.Utilities.System
{
    public class WpfScreen
    {
        private readonly Display _display;

        internal WpfScreen(Display display)
        {
            _display = display;
        }

        public static WpfScreen GetScreenFrom(Window window)
        {
            var windowInteropHelper = new WindowInteropHelper(window);
            var display = Display.FromHandle(windowInteropHelper.Handle);
            return new WpfScreen(display);
        }

        public Size DisplaySize => new Size(_display.Bounds.Size.Width, _display.Bounds.Size.Height);
    }
}
