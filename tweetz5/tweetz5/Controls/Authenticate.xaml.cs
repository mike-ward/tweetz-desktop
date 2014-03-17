// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Windows;
using tweetz5.Commands;
using tweetz5.Model;
using Settings = tweetz5.Properties.Settings;

namespace tweetz5.Controls
{
    public partial class Authenticate
    {
        private Twitter.OAuthTokens Tokens { get; set; }

        public Authenticate()
        {
            InitializeComponent();
        }

        private async void GetPin_OnClick(object sender, RoutedEventArgs e)
        {
            Tokens = await Twitter.GetRequestToken();
            var url = "https://api.twitter.com/oauth/authenticate?oauth_token=" + Tokens.OAuthToken;
            OpenLinkCommand.Command.Execute(url, this);
        }

        private async void SignIn_OnClick(object sender, RoutedEventArgs e)
        {
            var tokens = await Twitter.GetAccessToken(Tokens.OAuthToken, Tokens.OAuthSecret, Pin.Text);
            Pin.Text = string.Empty;
            Settings.Default.AccessToken = tokens.OAuthToken;
            Settings.Default.AccessTokenSecret = tokens.OAuthSecret;
            Settings.Default.UserId = tokens.UserId;
            Settings.Default.ScreenName = tokens.ScreenName;
            Settings.Default.Save();
            SignInCommand.Command.Execute(null, this);
        }
    }
}