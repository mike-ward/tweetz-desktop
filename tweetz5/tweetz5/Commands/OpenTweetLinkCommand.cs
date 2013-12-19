using System.Windows;
using System.Windows.Input;
using tweetz5.Controls;
using tweetz5.Model;

namespace tweetz5.Commands
{
    public class OpenTweetLinkCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private OpenTweetLinkCommand()
        {
        }

        public static void CommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            var tweet = (Tweet) ea.Parameter ?? mainWindow.Timeline.GetSelectedTweet;
            var link = TimelineController.TweetLink(tweet);
            MyCommands.OpenLinkCommand.Execute(link, mainWindow);
        }
    }
}