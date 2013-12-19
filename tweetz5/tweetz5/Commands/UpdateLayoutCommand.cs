using System.Windows.Input;

namespace tweetz5.Commands
{
    internal class UpdateLayoutCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();

        private UpdateLayoutCommand()
        {
        }
    }
}