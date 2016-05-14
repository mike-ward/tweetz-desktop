using System;
using System.Threading.Tasks;
using System.Windows;

namespace tweetz5.Utilities
{
    internal static class Run
    {
        public static void Later(Action action, int delay = 100)
        {
            Task.Delay(delay).ContinueWith(t => Application.Current.Dispatcher.InvokeAsync(action));
        }
    }
}