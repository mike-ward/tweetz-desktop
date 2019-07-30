using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using tweetz5.Properties;
using tweetz5.Utilities.ExceptionHandling;
using tweetz5.Utilities.Translate;

namespace tweetz5
{
    public partial class App
    {
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;
            AppDomain.CurrentDomain.UnhandledException += (o, args) => ShowCrashReport((Exception)args.ExceptionObject);
            Current.DispatcherUnhandledException += (o, args) => ShowCrashReport(args.Exception);
            TaskScheduler.UnobservedTaskException += (o, args) => ShowCrashReport(args.Exception);

            TranslationService.Instance.TranslationProvider = new TranslationProviderNameValueFile();

            try
            {
                if (Settings.Default.UpgradeSettings)
                {
                    Settings.Default.Upgrade();
                    Settings.Default.UpgradeSettings = false;
                    Settings.Default.Save();
                }
            }
            catch (ConfigurationException ex)
            {
                MessageBox.Show("Tweetz User settings are corrupted. Click OK and restart Tweetz-Desktop to correct the problem.");
                if (ex.InnerException is ConfigurationException configurationException) File.Delete(configurationException.Filename);
                else MessageBox.Show("Tweetz could not correct the problem. Settings file must be manually deleted");
                Environment.Exit(1);
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