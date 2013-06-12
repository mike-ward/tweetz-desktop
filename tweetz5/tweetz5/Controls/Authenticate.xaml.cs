using System;
using System.Windows;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public partial class Authenticate
    {
        public Authenticate()
        {
            InitializeComponent();
        }

        private void GetPin_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var tokens = Twitter.GetRequestToken();
                var url = "https://api.twitter.com/oauth/authenticate?oauth_token=" + tokens.OAuthToken;
                MainWindow.OpenLinkCommand.Execute(url, this);
            }
            catch (Exception ex)
            {
                MainWindow.AlertCommand.Execute(ex.Message, this);
            }
        }

        private void Authorize_OnClick(object sender, RoutedEventArgs e)
        {
            var tokens = Twitter.GetAccessToken(Pin.Text);
            // save tokens
            // wakeup timeline
        }
    }
}