using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    internal class SearchCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private SearchCommand()
        {
        }

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var query = ea.Parameter as string;
            if (string.IsNullOrWhiteSpace(query)) return;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.Timeline.SearchControl.SetSearchText(query);
            SwitchTimelinesCommand.Command.Execute(Timelines.SearchName, mainWindow);
            mainWindow.Timeline.Controller.Search(query);
        }
    }
}