// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Globalization;
using System.Windows.Data;
using tweetz5.Model;
using tweetz5.Utilities.Translate;

namespace tweetz5.Controls
{
    public partial class AboutUser
    {
        public string ScreenName { private get; set; }

        public AboutUser()
        {
            InitializeComponent();
            Opened += async (sender, args) => 
            {
                var user = await Twitter.GetUserInformation(ScreenName);
                var friendship = await Twitter.Friendship(ScreenName);
                user.Following = friendship.Following;
                user.FollowedBy = friendship.FollowedBy;
                if (user.Entities != null && user.Entities.Url != null && user.Entities.Url.Urls != null && user.Entities.Url.Urls[0] != null)
                {
                    user.Url = user.Entities.Url.Urls[0].ExpandedUrl;
                }
                DataContext = user;
            };
        }
    }

    [ValueConversion(typeof (bool), typeof (string))]
    public class BoolToFollowingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value
                ? TranslationService.Instance.Translate("profile_following")
                : TranslationService.Instance.Translate("profile_follow");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof (bool), typeof (string))]
    public class BoolToUnfollowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value
                ? TranslationService.Instance.Translate("profile_unfollow")
                : TranslationService.Instance.Translate("profile_follow");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }

    [ValueConversion(typeof (bool), typeof (string))]
    public class BoolToFollowedByConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool) value ? TranslationService.Instance.Translate("profile_follows_you") : "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}