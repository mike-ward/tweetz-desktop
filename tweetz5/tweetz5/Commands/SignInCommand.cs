using System.Windows;
using System.Windows.Input;
using tweetz5.Model;
using Settings = tweetz5.Properties.Settings;

namespace tweetz5.Commands
{
    public static class SignInCommand
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
                if (Application.Current != null)
                {
                    Application.Current.Dispatcher.Invoke(
                        () => SwitchTimelinesCommand.Command.Execute(View.Unified, Application.Current.MainWindow));
                }
            }
        }
    }
}