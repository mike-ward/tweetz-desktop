// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Globalization;
using System.Windows.Data;

namespace tweetz5.Utilities
{
    [ValueConversion(typeof (string), typeof (string))]
    internal class ScreenNameToLinkConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Format("https://twitter.com/{0}", value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}