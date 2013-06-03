// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Diagnostics;
using System.Media;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Threading;
using tweetz5.Model;

namespace tweetz5
{
    public partial class MainWindow
    {
        public static readonly RoutedCommand ReplyCommand = new RoutedUICommand();
        public static readonly RoutedCommand RetweetCommand = new RoutedUICommand();
        public static readonly RoutedCommand RetweetClassicCommand = new RoutedUICommand();
        public static readonly RoutedCommand FavoritesCommand = new RoutedUICommand();
        public static readonly RoutedCommand CopyCommand = new RoutedUICommand();
        public static readonly RoutedCommand UpdateStatusHomeTimelineCommand = new RoutedUICommand();
        public static readonly RoutedCommand SwitchTimelinesCommand = new RoutedUICommand();
        public static readonly RoutedCommand ShowUserInformationCommand = new RoutedUICommand();
        public static readonly RoutedCommand OpenLinkCommand = new RoutedUICommand();
        public static readonly RoutedCommand FollowUserComand = new RoutedUICommand();

        private DispatcherTimer _switchTimelineTimer;

        public MainWindow()
        {
            InitializeComponent();
            // HACK: Compose.Toggle does not work the first time unless the control is initially visible.
            Loaded += (sender, args) => _compose.Visibility = Visibility.Collapsed;
            SetButtonStates("unified");
        }

        private void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ThumbOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Height = Math.Max(Height + e.VerticalChange, MinHeight);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            _timeline.Height = e.NewSize.Height - _topbar.ActualHeight - _navBar.ActualHeight - _compose.ActualHeight - _resizeBar.ActualHeight;
        }

        private void ComposeOnClick(object sender, RoutedEventArgs e)
        {
            _compose.Toggle();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), false, true));
            if (_compose.IsVisible)
            {
                _compose.Focus();
            }
        }

        private void SwitchTimeslinesCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            // HACK: The timeline takes a precievable amount of time to update
            // when switching timelines. Improve UI feedback by immediatelly hiding
            // timeline and launching a timer to update it. Hiding without the timer
            // does not give the desired effect.
            if (_switchTimelineTimer == null)
            {
                _switchTimelineTimer = new DispatcherTimer { Interval = new TimeSpan(1) };
                _switchTimelineTimer.Tick += (o, args) =>
                {
                    _switchTimelineTimer.Stop();
                    _timeline.Controller.SwitchTimeline(_switchTimelineTimer.Tag as string);
                    _timeline.Visibility = Visibility.Visible;
                };                
            }
            _timeline.Visibility = Visibility.Hidden;
            _switchTimelineTimer.Tag = ea.Parameter;
            SetButtonStates(ea.Parameter as string);
            _switchTimelineTimer.Start();
        }

        private void SetButtonStates(string timeline)
        {
            _unifiedButton.IsEnabled = timeline != "unified";
            _homeButton.IsEnabled = timeline != "home";
            _mentionsButton.IsEnabled = timeline != "mentions";
            _messagesButton.IsEnabled = timeline != "messages";
            _favoritesButton.IsEnabled = timeline != "favorites";
        }

        private void CopyTweetCommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            Clipboard.SetText(tweet.Text);
        }

        private void ReplyCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            var message = string.Format("@{0} ", tweet.ScreenName);
            _compose.Show(message, tweet.StatusId);
        }

        private void RetweetCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                var tweet = (Tweet)ea.Parameter;
                var json = Twitter.RetweetStatus(tweet.StatusId);
                if (json.Contains(tweet.StatusId))
                {
                    var status = Status.ParseJson("[" + json + "]");
                    tweet.RetweetedBy = Timelines.RetweetedBy(status[0]);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RetweetClassicCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            var message = string.Format("RT @{0} {1}", tweet.ScreenName, tweet.Text);
            _compose.Show(message);
        }

        private void FavoritesCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                var tweet = (Tweet)ea.Parameter;
                var json = tweet.Favorited ? Twitter.DestroyFavorite(tweet.StatusId) : Twitter.CreateFavorite(tweet.StatusId);
                if (json.Contains(tweet.StatusId)) tweet.Favorited = !tweet.Favorited;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UpdateStatusHomeTimelineHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var statuses = (Status[])ea.Parameter;
            _timeline.Controller.UpdateStatus(statuses);
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ShowUserInformationCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            _userInformationPopup.ScreenName = ea.Parameter as string;
            _userInformationPopup.IsOpen = true;
        }

        private void OpenLinkCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            Process.Start(new ProcessStartInfo((string)ea.Parameter));
            ea.Handled = true;
        }

        private void FollowCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var user = (User)ea.Parameter;
            user.Following = user.Following ? !Twitter.Unfollow(user.ScreenName) : Twitter.Follow(user.ScreenName);
            ea.Handled = true;
        }

        private void NotifyCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var player = new SoundPlayer {Stream = Properties.Resources.Notify};
            player.Play();
        }
    }
}