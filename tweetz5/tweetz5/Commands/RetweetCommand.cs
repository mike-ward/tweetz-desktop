using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    public static class RetweetCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            var tweet = ea.Parameter as Tweet ?? mainWindow.Timeline.GetSelectedTweet;
            if (tweet != null) mainWindow.Timeline.Controller.Retweet(tweet);
        }
    }
}