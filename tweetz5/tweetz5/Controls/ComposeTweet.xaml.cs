// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public partial class ComposeTweet
    {
        private string _inReplyToId;

        public ComposeTweet()
        {
            InitializeComponent();
        }

        public void Show(string message = "", string inReplyToId = null)
        {
            _composeTitle.Text = "Compose a tweet";
            _textBox.Text = message;
            _inReplyToId = inReplyToId;
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            _textBox.Clear();
            Visibility = Visibility.Collapsed;
        }

        public void Toggle()
        {
            if (IsVisible) Hide();
            else Show();
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) Hide();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_textBox.IsVisible)
            {
                _textBox.Focus();
                _textBox.SelectionStart = _textBox.Text.Length;
            }
        }

        private void OnSend(object sender, RoutedEventArgs e)
        {
            try
            {
                _send.IsEnabled = false;
                var json = Twitter.UpdateStatus(_textBox.Text, _inReplyToId);
                if (json.Contains("id_str") == false)
                {
                    _composeTitle.Text = "Error";
                }
                else
                {
                    var status = Status.ParseJson("[" + json + "]");
                    MainWindow.UpdateStatusHomeTimeline.Execute(status, this);
                    Hide();
                }
            }
            catch (Exception)
            {
                _composeTitle.Text = "Error";
            }
            finally
            {
                _send.IsEnabled = true;
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