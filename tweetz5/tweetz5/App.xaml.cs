using System;
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
            AppDomain.CurrentDomain.UnhandledException += (o, args) => new CrashReport((Exception)args.ExceptionObject).ShowDialog();
            TranslationService.Instance.TranslationProvider = new TranslationProviderNameValueFile();

            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();
            }
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