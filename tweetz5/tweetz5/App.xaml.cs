// Copyright (c) 2012 Blue Onion Software. All rights reserved.

namespace tweetz5
{
    public partial class App
    {
        private void ApplicationExit(object sender, System.Windows.ExitEventArgs e)
        {
            tweetz5.Properties.Settings.Default.Save();
        }
    }
}