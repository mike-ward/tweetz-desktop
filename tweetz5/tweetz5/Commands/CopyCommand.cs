using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    public class CopyCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private CopyCommand()
        {
        }

        public static void CommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            var tweet = ea.Parameter as Tweet ?? mainWindow.Timeline.GetSelectedTweet;
            if (tweet == null) return;
            mainWindow.Timeline.Controller.CopyTweetToClipboard(tweet);
        }
    }
}