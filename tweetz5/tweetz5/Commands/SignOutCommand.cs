using System.Windows;
using System.Windows.Input;
using tweetz5.Properties;

namespace tweetz5.Commands
{
    internal class SignOutCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private SignOutCommand()
        {
        }

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.Timeline.Controller.StopTimelines();
            Settings.Default.AccessToken = "";
            Settings.Default.AccessTokenSecret = "";
            Settings.Default.ScreenName = "";
            Settings.Default.UserId = "";
            Settings.Default.Save();
            SignInCommand.Command.Execute(null, mainWindow);
        }
    }
}