using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using tweetz5.Commands;
using tweetz5.Model;
using Settings = tweetz5.Properties.Settings;

namespace tweetz5.Controls
{
    public partial class Authenticate : INotifyPropertyChanged
    {
        private Twitter.OAuthTokens _tokens;

        public Authenticate()
        {
            InitializeComponent();
        }

        public Twitter.OAuthTokens Tokens
        {
            get { return _tokens; }
            set
            {
                if (_tokens == value) return;
                _tokens = value;
                OnPropertyChanged();
            }
        }

        private async void GetPin_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Tokens = await Twitter.GetRequestToken();
                var url = "https://api.twitter.com/oauth/authenticate?oauth_token=" + Tokens.OAuthToken;
                OpenLinkCommand.Command.Execute(url, this);
            }
            catch (AggregateException ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        private async void SignIn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var tokens = await Twitter.GetAccessToken(Tokens.OAuthToken, Tokens.OAuthSecret, Pin.Text);
                Pin.Text = string.Empty;
                if (tokens == null) return;
                Settings.Default.AccessToken = tokens.OAuthToken;
                Settings.Default.AccessTokenSecret = tokens.OAuthSecret;
                Settings.Default.UserId = tokens.UserId;
                Settings.Default.ScreenName = tokens.ScreenName;
                Settings.Default.Save();
                SignInCommand.Command.Execute(null, this);
            }
            catch (AggregateException ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}