using System.Windows;
using System.Windows.Input;

namespace tweetz5.Commands
{
    public class SetFontSizeCommand
    {
        public static readonly RoutedCommand Command = new RoutedCommand();

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var size = double.Parse(ea.Parameter.ToString());
            Application.Current.Resources["AppFontSize"] = size;
            Application.Current.Resources["AppFontSizePlus1"] = size + 1;
            Application.Current.Resources["AppFontSizePlus3"] = size + 3;
            Application.Current.Resources["AppFontSizePlus7"] = size + 7;
            Application.Current.Resources["AppFontSizeMinus1"] = size - 1;
        }
    }
}