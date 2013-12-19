using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    public class SwitchTimelinesCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private SwitchTimelinesCommand()
        {
        }

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var timelineName = (string) ea.Parameter;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.Compose.Visibility = Visibility.Collapsed;
            mainWindow.SettingsPanel.Visibility = Visibility.Collapsed;
            mainWindow.BottomResizeBar.Visibility = Visibility.Visible;
            mainWindow.Timeline.Visibility = Visibility.Visible;
            mainWindow.MainPanel.Visibility = Visibility.Visible;
            mainWindow.SetButtonStates(timelineName);
            mainWindow.Timeline.Controller.SwitchTimeline(timelineName);
            mainWindow.Timeline.ScrollToTop();
            mainWindow.Timeline.Focus();
        }
    }
}