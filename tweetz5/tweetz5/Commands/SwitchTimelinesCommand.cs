using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    public static class SwitchTimelinesCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var view = (View) ea.Parameter;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.Compose.Visibility = Visibility.Collapsed;
            mainWindow.SettingsPanel.Visibility = Visibility.Collapsed;
            mainWindow.BottomResizeBar.Visibility = Visibility.Visible;
            mainWindow.Timeline.Visibility = Visibility.Visible;
            mainWindow.MainPanel.Visibility = Visibility.Visible;
            mainWindow.SetButtonStates(view);
            mainWindow.Timeline.Controller.SwitchTimeline(view);
            mainWindow.Timeline.ScrollToTop();
            mainWindow.Timeline.Focus();
        }
    }
}