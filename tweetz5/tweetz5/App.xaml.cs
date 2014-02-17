// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Windows;
using tweetz5.Properties;
using tweetz5.Utilities.Translate;

namespace tweetz5
{
    public partial class App
    {
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
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