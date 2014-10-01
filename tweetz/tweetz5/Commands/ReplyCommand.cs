using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using tweetz5.Model;
using Settings = tweetz5.Properties.Settings;

namespace tweetz5.Commands
{
    public static class ReplyCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            var tweet = ea.Parameter as Tweet ?? mainWindow.Timeline.GetSelectedTweet;
            if (tweet == null) return;
            if (tweet.IsDirectMessage)
            {
                mainWindow.Compose.ShowDirectMessage(tweet.ScreenName);
            }
            else
            {
                var matches = new Regex(@"(?<=^|(?<=[^a-zA-Z0-9-_\.]))@([A-Za-z]+[A-Za-z0-9_]+)")
                    .Matches(tweet.Text);

                var names = matches
                    .Cast<Match>()
                    .Where(m => m.Groups[1].Value != tweet.ScreenName)
                    .Where(m => m.Groups[1].Value != Settings.Default.ScreenName)
                    .Select(m => "@" + m.Groups[1].Value)
                    .Distinct();

                var replyTos = string.Join(" ", names);
                var message = string.Format("@{0} {1}{2}", tweet.ScreenName, replyTos, (replyTos.Length > 0) ? " " : "");
                mainWindow.Compose.Show(message, tweet.StatusId);
            }
        }
    }
}