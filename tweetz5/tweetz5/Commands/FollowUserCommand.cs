using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Commands
{
    internal static class FollowUserCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var user = (User) ea.Parameter;
            user.Following = user.Following ? !Twitter.Unfollow(user.ScreenName) : Twitter.Follow(user.ScreenName);
        }
    }
}