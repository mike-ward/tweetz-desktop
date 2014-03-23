using System.Media;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using tweetz5.Properties;
using tweetz5.Utilities.ExceptionHandling;

namespace tweetz5.Commands
{
    internal static class ChirpCommand
    {
        public static readonly RoutedCommand Command = new RoutedUICommand();
        private static bool _playing;

        public static void CommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            if (Settings.Default.Chirp == false || _playing) return;

            _playing = true;
            Task.Run(() =>
            {
                var player = new SoundPlayer {Stream = Resources.Notify};
                player.PlaySync();
                Thread.Sleep(200);
                _playing = false;
            }).LogAggregateExceptions();
        }
    }
}