using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    internal static class UpdateStatusHomeTimelineCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var statuses = (Status[]) ea.Parameter;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.Timeline.Controller.UpdateStatus(new[] {Timelines.HomeName, Timelines.UnifiedName}, statuses, "h");
        }
    }
}