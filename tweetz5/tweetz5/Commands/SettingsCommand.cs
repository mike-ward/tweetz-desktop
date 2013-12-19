using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    internal class SettingsCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private SettingsCommand()
        {
        }

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.SettingsPanel.Visibility = Visibility.Visible;
            mainWindow.Timeline.Visibility = Visibility.Collapsed;
            mainWindow.SetButtonStates("settings");
        }
    }
}