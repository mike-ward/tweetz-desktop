// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using tweetz5.Annotations;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public partial class ComposeTweet : INotifyPropertyChanged
    {
        private string _inReplyToId;
        private bool _directMessage;
        private string _image;

        public ComposeTweet()
        {
            InitializeComponent();
            DataContext = this;
            SizeChanged += (sender, args) => MainWindow.UpdateLayoutCommand.Execute(null, this);
        }

        public string Image
        {
            get { return _image; }
            set
            {
                if (_image != value)
                {
                    _image = value;
                    OnPropertyChanged();
                }
            }
        }

        public void Show(string message = "", string inReplyToId = null)
        {
            ComposeTitle.Text = "Compose a tweet";
            TextBox.Text = message;
            _directMessage = false;
            _inReplyToId = inReplyToId;
            SendButtonText.Text = "Tweet";
            Image = null;
            Visibility = Visibility.Visible;
        }

        public void ShowDirectMessage(string name, string screenName)
        {
            ComposeTitle.Text = name;
            TextBox.Text = string.Empty;
            _directMessage = true;
            _inReplyToId = null;
            SendButtonText.Text = "Send";
            Image = null;
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
                string json;

                if (_directMessage)
                {
                    json = Twitter.SendDirectMessage(TextBox.Text, _inReplyToId);
                }
                else
                {
                    json = string.IsNullOrWhiteSpace(Image) 
                        ? Twitter.UpdateStatus(TextBox.Text, _inReplyToId) 
                        : Twitter.UpdateStatusWithMedia(TextBox.Text, Image);
                }

                if (json.Contains("id_str"))
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

        private void OnPhoto(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog
            {
                CheckFileExists = true,
                Filter = "Images (*.png, *.jpg, *.jpeg, *.gif)|*.png;*.jpg;*.jpeg;*.gif"
            };
            if (dialog.ShowDialog() == true)
            {
                Image = dialog.FileName;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
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