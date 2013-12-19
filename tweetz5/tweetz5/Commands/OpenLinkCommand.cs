using System.Diagnostics;
using System.Windows.Input;

namespace tweetz5.Commands
{
    internal class OpenLinkCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private OpenLinkCommand()
        {
        }

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Process.Start(new ProcessStartInfo((string) ea.Parameter));
        }
    }
}