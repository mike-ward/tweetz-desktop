// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;
using tweetz5.Commands;
using tweetz5.Model;
using tweetz5.Utilities.Translate;
using Settings = tweetz5.Properties.Settings;

namespace tweetz5.Controls
{
    public sealed partial class ComposeTweet : INotifyPropertyChanged
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
            SizeChanged += (sender, args) => UpdateLayoutCommand.Command.Execute(null, this);
        }

        public string Image
        {
            get { return _image; }
            set
            {
                if (_image == value) return;
                _image = value;
                OnPropertyChanged();
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
            TextBox.SpellCheck.IsEnabled = Settings.Default.SpellCheck;
            Visibility = Visibility.Visible;
        }

        public void ShowDirectMessage(string name)
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

        private void Hide()
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
                if (_friendsFilter == value) return;
                _friendsFilter = value;
                // ReSharper disable once ExplicitCallerInfoArgument
                OnPropertyChanged("Friends");
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
                if (_friends.Equals(value)) return;
                _friends = value;
                OnPropertyChanged();
            }
        }

        private static char ConvertKeyCodeToChar(Key key)
        {
            if (key == Key.D2 && Keyboard.Modifiers == ModifierKeys.Shift) return '@';
            var letter = key - Key.A;
            if (letter >= 0 && letter <= 26) return (char) ('a' + letter);
            var digit = key - Key.D0;
            if (digit >= 0 && digit <= 9) return (char) ('0' + digit);
            return default(char);
        }

        private bool _atPressed;

        private void OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (_atPressed) CloseFriendsPopup();
                else Hide();
                return;
            }

            var ch = ConvertKeyCodeToChar(e.Key);

            if (ch == '@')
            {
                _atPressed = true;
                return;
            }

            if (_atPressed == false) return;

            switch (e.Key)
            {
                case Key.Back:
                    var length = FriendsFilter.Length;
                    if (length > 0) FriendsFilter = FriendsFilter.Remove(length - 1);
                    else CloseFriendsPopup();
                    break;

                case Key.Up:
                    if (FriendsListBox.SelectedIndex > 0)
                    {
                        FriendsListBox.SelectedIndex -= 1;
                        e.Handled = true;
                    }
                    break;

                case Key.Down:
                    if (FriendsListBox.SelectedIndex < FriendsListBox.Items.Count - 1)
                    {
                        FriendsListBox.SelectedIndex += 1;
                        e.Handled = true;
                    }
                    break;

                case Key.Return:
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
                    break;

                default:
                    if (Char.IsLetterOrDigit(ch))
                    {
                        FriendsPopup.IsOpen = true;
                        FriendsFilter += ch;
                    }
                    else if (ch != default(char))
                    {
                        CloseFriendsPopup();
                    }
                    break;
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

        private
            void OnIsVisibleChanged
            (object sender, DependencyPropertyChangedEventArgs e)
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
                    Hide();
                    var status = Status.ParseJson("[" + json + "]");
                    UpdateStatusHomeTimelineCommand.Command.Execute(status, this);
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}