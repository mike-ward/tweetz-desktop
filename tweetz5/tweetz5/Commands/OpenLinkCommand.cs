using System.Diagnostics;
using System.Windows.Input;

namespace tweetz5.Commands
{
    internal static class OpenLinkCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Process.Start(new ProcessStartInfo((string) ea.Parameter));
        }
    }
}