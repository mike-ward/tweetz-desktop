using System;
using System.Globalization;
using System.Windows.Data;
using tweetz5.Utilities.System;

namespace tweetz5.Utilities
{
    public class Win7FontFamilyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return !BuildInfo.IsWindows8OrNewer ? "Segoe UI Symbol" : "FontAwesome";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}