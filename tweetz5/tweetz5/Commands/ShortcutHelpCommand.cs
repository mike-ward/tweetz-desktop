using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    internal static class ShortcutHelpCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.ShortcutHelp.IsOpen = true;
        }
    }
}