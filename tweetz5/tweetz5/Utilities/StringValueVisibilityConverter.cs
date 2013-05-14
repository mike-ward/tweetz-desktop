// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace tweetz5.Utilities
{
    [ValueConversion(typeof (string), typeof (Visibility))]
    public class StringValueVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}