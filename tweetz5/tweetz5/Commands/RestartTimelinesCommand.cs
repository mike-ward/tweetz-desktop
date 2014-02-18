using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    internal static class RestartTimelinesCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.Timeline.Controller.StopTimelines();
            Task.Run(() => mainWindow.Timeline.Controller.StartTimelines());
        }
    }
}