using System.Windows.Input;

namespace tweetz5
{
    internal static class MyCommands
    {
        public static readonly RoutedCommand OpenTweetLinkCommand = new RoutedUICommand();
        public static readonly RoutedCommand UpdateStatusHomeTimelineCommand = new RoutedUICommand();
        public static readonly RoutedCommand SwitchTimelinesCommand = new RoutedUICommand();
        public static readonly RoutedCommand ShowUserInformationCommand = new RoutedUICommand();
        public static readonly RoutedCommand OpenLinkCommand = new RoutedUICommand();
        public static readonly RoutedCommand FollowUserComand = new RoutedUICommand();
        public static readonly RoutedCommand SearchCommand = new RoutedUICommand();
        public static readonly RoutedCommand AlertCommand = new RoutedUICommand();
        public static readonly RoutedCommand SignOutCommand = new RoutedUICommand();
        public static readonly RoutedCommand SettingsCommand = new RoutedUICommand();
        public static readonly RoutedCommand UpdateLayoutCommand = new RoutedUICommand();
        public static readonly RoutedCommand OpenComposeCommand = new RoutedUICommand();
        public static readonly RoutedCommand ShortcutHelpCommand = new RoutedUICommand();
    }
}