using System;
using System.Globalization;
using System.Windows.Data;
using tweetz5.Model;

namespace tweetz5.Utilities
{
    public class UniformGridRowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var links = value as Media[];
            return links == null || links.Length < 3 ? 1 : 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
