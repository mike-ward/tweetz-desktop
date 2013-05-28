// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public partial class AboutUser
    {
        public string ScreenName { get; set; }

        public AboutUser()
        {
            InitializeComponent();
            Opened += (sender, args) =>
            {
                DataContext = new User { ProfileImageUrl = "../Assets/Images/waiting.png", Url="http://twitter.com" };
                Task.Run(() =>
                {
                    var user = Twitter.GetUserInformation(ScreenName);
                    if (user.Entities != null && user.Entities.Url != null && user.Entities.Url.Urls != null && user.Entities.Url.Urls[0] != null)
                    {
                        user.Url = user.Entities.Url.Urls[0].ExpandedUrl;
                    }
                    Application.Current.Dispatcher.Invoke(() => DataContext = user);
                });
            };
        }

        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.ToString()));
            e.Handled = true;
        }
    }
}