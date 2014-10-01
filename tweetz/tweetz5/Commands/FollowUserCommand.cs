using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    internal static class FollowUserCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public async static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var user = (User) ea.Parameter;
            user.Following = user.Following ? !await Twitter.Unfollow(user.ScreenName) : await Twitter.Follow(user.ScreenName);
        }
    }
}