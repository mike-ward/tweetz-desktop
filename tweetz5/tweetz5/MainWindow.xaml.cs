// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
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

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) => _compose.Visibility = Visibility.Collapsed;
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

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void SwitchTimeslinesCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
        {
            _timeline.Controller.SwitchTimeline(ea.Parameter as string);
        }

        private void CopyTweetCommand(object target, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            Clipboard.SetText(tweet.Text);
        }

        private void ReplyCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            var message = string.Format("@{0} ", tweet.ScreenName);
            _compose.Show(message, tweet.StatusId);
        }

        private void RetweetCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
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

        private void RetweetClassicCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            var message = string.Format("RT @{0} {1}", tweet.ScreenName, tweet.Text);
            _compose.Show(message);
        }

        private void FavoritesCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
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

        private void UpdateStatusHomeTimelineExecuted(object sender, ExecutedRoutedEventArgs ea)
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
    }
}