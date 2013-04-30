// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace tweetz5.Controls
{
    public partial class ComposeTweet
    {
        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public ComposeTweet()
        {
            InitializeComponent();
            _timer.Interval = new TimeSpan(1);
            _timer.Tick += (sender, args) =>
            {
                _timer.Stop();
                _textBox.Focus();
                _textBox.SelectionStart = _textBox.SelectionStart = _textBox.Text.Length;
            };
        }

        private void OnGotFocus(object sender, RoutedEventArgs e)
        {
            _timer.Start();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                _textBox.Clear();
                Visibility = Visibility.Collapsed;
            }
        }
    }

    public class LengthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = (int)value;
            return length > 140 ? Brushes.Red : (length > 134 ? Brushes.SandyBrown : Brushes.WhiteSmoke);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}