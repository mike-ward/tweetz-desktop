// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Globalization;
using System.Windows.Data;

namespace tweetz5.Utilities
{
    [ValueConversion(typeof (string), typeof (string))]
    public class ScreenNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var name = value as string;
            if (name != null) return "@" + name;
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}