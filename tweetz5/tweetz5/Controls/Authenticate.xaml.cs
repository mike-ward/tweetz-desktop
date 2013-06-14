using System;
using System.Windows;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public partial class Authenticate
    {
        private Twitter.OAuthTokens Tokens { get; set; }

        public Authenticate()
        {
            InitializeComponent();
        }

        private void GetPin_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Tokens = Twitter.GetRequestToken();
                var url = "https://api.twitter.com/oauth/authenticate?oauth_token=" + Tokens.OAuthToken;
                MainWindow.OpenLinkCommand.Execute(url, this);
            }
            catch (Exception ex)
            {
                MainWindow.AlertCommand.Execute(ex.Message, this);
            }
        }

        private void Authorize_OnClick(object sender, RoutedEventArgs e)
        {
            var tokens = Twitter.GetAccessToken(Tokens.OAuthToken, Tokens.OAuthSecret, Pin.Text);
            Properties.Settings.Default.AccessToken = tokens.OAuthToken;
            Properties.Settings.Default.AccessTokenSecret = tokens.OAuthSecret;
            Properties.Settings.Default.UserId = tokens.UserId;
            Properties.Settings.Default.ScreenName = tokens.ScreenName;
        }
    }
}