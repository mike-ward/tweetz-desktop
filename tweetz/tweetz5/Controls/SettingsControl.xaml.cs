using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Navigation;

namespace tweetz5.Controls
{
    public partial class SettingsControl
    {
        public SettingsControl()
        {
            InitializeComponent();
        }

        private void DonateHyperLink(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri));
            e.Handled = true;
        }
    }

    public class ThemeToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && value.Equals(true) ? parameter : "";
        }
    }
}