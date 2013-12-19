using System.Windows;
using System.Windows.Input;
using tweetz5.Properties;

namespace tweetz5.Commands
{
    public class SignInCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.SettingsPanel.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(Settings.Default.UserId))
            {
                mainWindow.AuthenticatePanel.Visibility = Visibility.Visible;
                mainWindow.MainPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                mainWindow.AuthenticatePanel.Visibility = Visibility.Collapsed;
                mainWindow.Timeline.Controller.StartTimelines();
                mainWindow.Compose.Friends = mainWindow.Timeline.Controller.ScreenNames;
            }
        }
    }
}