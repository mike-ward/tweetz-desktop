using System;
using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    internal static class AlertCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var message = ea.Parameter as string;
            if (string.IsNullOrWhiteSpace(message)) return;
            var mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow == null) throw new ArgumentNullException();
            mainWindow.StatusAlert.Message.Text = message;
            mainWindow.StatusAlert.IsOpen = true;
        }
    }
}