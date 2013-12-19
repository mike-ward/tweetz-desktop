using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    public class RetweetClassicCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            var tweet = (Tweet) ea.Parameter ?? mainWindow.Timeline.GetSelectedTweet;
            var message = string.Format("RT @{0} {1}", tweet.ScreenName, tweet.Text);
            mainWindow.Compose.Show(message);
        }
    }
}