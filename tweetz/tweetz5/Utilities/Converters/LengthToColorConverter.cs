using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace tweetz5.Controls
{
    public class LengthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = (int) value;
            var brush = (Brush) Application.Current.FindResource("ComposeCharacterCounterForegroundBrush");
            return length > 140 ? Brushes.Red : (length > 134 ? Brushes.SandyBrown : brush);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}