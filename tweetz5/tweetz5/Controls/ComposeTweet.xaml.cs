// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.Win32;
using tweetz5.Annotations;
using tweetz5.Model;
using tweetz5.Utilities.Translate;

namespace tweetz5.Controls
{
    public partial class ComposeTweet : INotifyPropertyChanged
    {
        private string _inReplyToId;
        private bool _directMessage;
        private string _image;
        private IInputElement _previousFocusedElement;
        private IEnumerable<string> _friends = new string[0];

        public ComposeTweet()
        {
            InitializeComponent();
            DataContext = this;
            SizeChanged += (sender, args) => MyCommands.UpdateLayoutCommand.Execute(null, this);
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
            CloseFriendsPopup();
            _previousFocusedElement = Keyboard.FocusedElement;
            ComposeTitle.Text = TranslationService.Instance.Translate("compose_title_tweet") as string;
            TextBox.Text = message;
            _directMessage = false;
            _inReplyToId = inReplyToId;
            SendButtonText.Text = TranslationService.Instance.Translate("compose_send_button_tweet") as string;
            Image = null;
            TextBox.SpellCheck.IsEnabled = Properties.Settings.Default.SpellCheck;
            Visibility = Visibility.Visible;
        }

        public void ShowDirectMessage(string name, string screenName)
        {
            _previousFocusedElement = Keyboard.FocusedElement;
            ComposeTitle.Text = name;
            TextBox.Text = string.Empty;
            _directMessage = true;
            _inReplyToId = null;
            SendButtonText.Text = TranslationService.Instance.Translate("compose_send_button_message") as string;
            Image = null;
            Visibility = Visibility.Visible;
        }

        public void Hide()
        {
            TextBox.Clear();
            CloseFriendsPopup();
            Visibility = Visibility.Collapsed;
            Keyboard.Focus(_previousFocusedElement);
        }

        public void Toggle()
        {
            if (IsVisible) Hide();
            else Show();
        }

        private string _friendsFilter = string.Empty;

        private string FriendsFilter
        {
            get { return _friendsFilter; }
            set
            {
                if (_friendsFilter != value)
                {
                    _friendsFilter = value;
                    OnPropertyChanged("Friends");
                }
            }
        }

        public IEnumerable<string> Friends
        {
            get
            {
                var friends = _friends
                    .Where(f => string.IsNullOrWhiteSpace(FriendsFilter) || f.StartsWith(FriendsFilter, StringComparison.CurrentCultureIgnoreCase))
                    .OrderBy(f => f)
                    .Select(f => "@" + f);
                return friends;
            }
            set
            {
                if (_friends.Equals(value) == false)
                {
                    _friends = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _atPressed;

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (_atPressed && e.Key == Key.Return)
            {
                if (FriendsListBox.SelectedIndex == -1 && FriendsListBox.Items.Count == 1)
                {
                    FriendsListBox.SelectedIndex = 0;
                }
                if (FriendsListBox.SelectedIndex != -1)
                {
                    InsertSelectedFriend();
                }
                CloseFriendsPopup();
                e.Handled = true;
            }
        }

        private void InsertSelectedFriend()
        {
            var index = TextBox.CaretIndex - FriendsFilter.Length - 1;
            var text = FriendsListBox.SelectedItem + " ";
            TextBox.Text = TextBox.Text.Remove(index, FriendsFilter.Length + 1);
            TextBox.Text = TextBox.Text.Insert(index, text);
            TextBox.CaretIndex = index + text.Length;
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_atPressed) CloseFriendsPopup();
                else Hide();
            }
            var ch = Utilities.System.NativeMethods.GetCharFromKey(e.Key);
            if (ch == '@')
            {
                _atPressed = true;
                return;
            }
            if (_atPressed)
            {
                if (Char.IsLetterOrDigit(ch))
                {
                    FriendsPopup.IsOpen = true;
                    FriendsFilter += ch;
                    return;
                }
                if (e.Key == Key.Back)
                {
                    var length = FriendsFilter.Length;
                    if (length > 0) FriendsFilter = FriendsFilter.Remove(length - 1);
                    else CloseFriendsPopup();
                    return;
                }
                if (e.Key == Key.Up)
                {
                    if (FriendsListBox.SelectedIndex > 0)
                    {
                        FriendsListBox.SelectedIndex -= 1;
                        e.Handled = true;
                    }
                    return;
                }
                if (e.Key == Key.Down)
                {
                    if (FriendsListBox.SelectedIndex < FriendsListBox.Items.Count - 1)
                    {
                        FriendsListBox.SelectedIndex += 1;
                        e.Handled = true;
                    }
                    return;
                }
                if (ch != default(char))
                {
                    CloseFriendsPopup();
                }
            }
        }

        private void FriendsListBoxOnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (FriendsListBox.SelectedIndex != -1)
            {
                e.Handled = true;
                InsertSelectedFriend();
                CloseFriendsPopup();
                TextBox.Focus();
            }
        }

        private void CloseFriendsPopup()
        {
            _atPressed = false;
            FriendsFilter = string.Empty;
            FriendsPopup.IsOpen = false;
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
                    MyCommands.UpdateStatusHomeTimelineCommand.Execute(status, this);
                    Hide();
                }
            }
            catch (Exception)
            {
                ComposeTitle.Text = TranslationService.Instance.Translate("compose_title_general_error") as string;
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
                ComposeTitle.Text = TranslationService.Instance.Translate("compose_title_shorten_error") as string;
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
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class LengthToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var length = (int) value;
            return length > 140 ? Brushes.Red : (length > 134 ? Brushes.SandyBrown : Brushes.WhiteSmoke);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}