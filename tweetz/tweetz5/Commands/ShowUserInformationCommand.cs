using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    public static class ShowUserInformationCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var mainWindow = (MainWindow) Application.Current.MainWindow;
            mainWindow.UserInformationPopup.ScreenName = ea.Parameter as string;
            mainWindow.UserInformationPopup.IsOpen = true;
        }
    }
}