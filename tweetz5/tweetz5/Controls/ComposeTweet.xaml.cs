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
            ComposeTitle.Text = "Compose a tweet";
            TextBox.Text = message;
            _inReplyToId = inReplyToId;
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            TextBox.Clear();
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
            if (TextBox.IsVisible)
            {
                TextBox.Focus();
                TextBox.SelectionStart = TextBox.Text.Length;
            }
        }

        private void OnSend(object sender, RoutedEventArgs e)
        {
            try
            {
                Send.IsEnabled = false;
                var json = Twitter.UpdateStatus(TextBox.Text, _inReplyToId);
                if (json.Contains("id_str") == false)
                {
                    ComposeTitle.Text = "Error";
                }
                else
                {
                    var status = Status.ParseJson("[" + json + "]");
                    MainWindow.UpdateStatusHomeTimelineCommand.Execute(status, this);
                    Hide();
                }
            }
            catch (Exception)
            {
                ComposeTitle.Text = "Error";
            }
            finally
            {
                Send.IsEnabled = true;
            }
        }

        private void OnShorten(object sender, RoutedEventArgs e)
        {
            try
            {
                Shorten.IsEnabled = false;
                TextBox.Text = ShortUrl.ShortenUrls(TextBox.Text);
            }
            catch (Exception)
            {
                ComposeTitle.Text = "Error shortening urls";
            }
            finally
            {
                Shorten.IsEnabled = true;
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