using System.Linq;
using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    public static class OpenFirstLinkInTweetCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var tweet = ea.Parameter as Tweet ?? mainWindow.Timeline.GetSelectedTweet;
            var link = tweet?.Urls.FirstOrDefault();
            if (link != null) OpenLinkCommand.Command.Execute(link, mainWindow);
        }
    }
}