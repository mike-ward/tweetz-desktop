using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    internal class AlertCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private AlertCommand()
        {
        }

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var message = ea.Parameter as string;
            if (string.IsNullOrWhiteSpace(message)) return;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.StatusAlert.Message.Text = message;
            mainWindow.StatusAlert.IsOpen = true;
        }
    }
}