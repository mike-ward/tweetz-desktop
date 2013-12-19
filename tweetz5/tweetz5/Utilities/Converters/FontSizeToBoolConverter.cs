using System;
using System.Globalization;
using System.Windows.Data;

namespace tweetz5.Utilities
{
    internal class FontSizeToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value is bool) ? parameter : null;
        }
    }
}