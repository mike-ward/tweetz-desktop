using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    public class DeleteTweetCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = ea.Parameter as Tweet;
            if (tweet == null) return;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.Timeline.Controller.DeleteTweet(tweet);
        }
    }
}