// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
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
                DataContext = new User {ProfileImageUrl = "../Assets/Images/waiting.png"};
                Task.Run(() =>
                {
                    var user = Twitter.GetUserInformation(ScreenName);
                    user.Following = Twitter.Following(ScreenName);
                    if (user.Entities != null && user.Entities.Url != null && user.Entities.Url.Urls != null && user.Entities.Url.Urls[0] != null)
                    {
                        user.Url = user.Entities.Url.Urls[0].ExpandedUrl;
                    }
                    Application.Current.Dispatcher.Invoke(() => DataContext = user);
                });
            };
        }
    }

    [ValueConversion(typeof (bool), typeof (string))]
    public class BoolToFollowingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Following" : "Follow";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToUnfollowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Unfollow!" : "Follow";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}