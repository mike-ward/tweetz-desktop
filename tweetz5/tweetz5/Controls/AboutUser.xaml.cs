// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
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
                DataContext = new User { ProfileImageUrl = "../Assets/Images/waiting.png" };
                Task.Run(() =>
                {
                    var user = Twitter.GetUserInformation(ScreenName);
                    Application.Current.Dispatcher.Invoke(() => DataContext = user);
                });
            };
        }
    }
}