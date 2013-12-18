using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;

namespace tweetz5.Controls
{
    public partial class SettingsControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }
    }

    public class ThemeToBooleanConverter: IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value.Equals(true)) ? parameter : "";
        }
    }
}