using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;
using tweetz5.Properties;
using tweetz5.Utilities.ExceptionHandling;
using tweetz5.Utilities.Translate;

namespace tweetz5
{
    public partial class App
    {
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            AppDomain.CurrentDomain.UnhandledException += (o, args) => ShowCrashReport((Exception)args.ExceptionObject);
            Current.DispatcherUnhandledException += (o, args) => ShowCrashReport(args.Exception);
            TaskScheduler.UnobservedTaskException += (o, args) => ShowCrashReport(args.Exception);

            TranslationService.Instance.TranslationProvider = new TranslationProviderNameValueFile();

            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();
            }
        }

        private static void ShowCrashReport(Exception exception)
        {
            var crashReport = new CrashReport(exception);
            MessageBox.Show(crashReport.Report);
            Environment.Exit(110);
        }

        private void AppSessionEnding(object sender, SessionEndingCancelEventArgs e)
        {
            Settings.Default.Save();
        }

        private void AppExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}