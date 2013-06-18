// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Windows;
using tweetz5.Properties;

namespace tweetz5
{
    public partial class App
    {
        private void ApplicationStart(object sender, StartupEventArgs e)
        {
            if (Settings.Default.UpgradeSettings)
            {
                Settings.Default.Upgrade();
                Settings.Default.UpgradeSettings = false;
                Settings.Default.Save();
            }
        }

        private void ApplicationExit(object sender, ExitEventArgs e)
        {
            Settings.Default.Save();
        }
    }
}