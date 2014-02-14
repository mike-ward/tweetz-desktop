using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    internal static class FavoritesCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            var tweet = ea.Parameter as Tweet ?? mainWindow.Timeline.GetSelectedTweet;
            if (tweet == null) return;
            if (tweet.Favorited) mainWindow.Timeline.Controller.RemoveFavorite(tweet);
            else mainWindow.Timeline.Controller.AddFavorite(tweet);
        }
    }
}