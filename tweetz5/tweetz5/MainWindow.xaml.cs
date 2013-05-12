// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
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
        public static readonly RoutedCommand UpdateStatusHomeTimeline = new RoutedUICommand();

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
            _timeline.Height = e.NewSize.Height - _topbar.ActualHeight - _compose.ActualHeight - _resizeBar.ActualHeight;
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

        private void CopyTweetCommand(object target, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            Clipboard.SetText(tweet.Text);
        }

        private void ReplyCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            var message = string.Format("@{0} {1}", tweet.ScreenName, tweet.Text);
            _compose.Show(message, tweet.StatusId);
        }

        private void RetweetCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void RetweetClassicCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            var message = string.Format("RT @{0} {1}", tweet.ScreenName, tweet.Text);
            _compose.Show(message);
        }

        private void FavoritesCommandExecuted(object sender, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            var json = tweet.Favorited ? Twitter.DestroyFavorite(tweet.StatusId) : Twitter.CreateFavorite(tweet.StatusId);
            if (json.Contains(tweet.StatusId)) tweet.Favorited = !tweet.Favorited;
        }

        private void UpdateStatusHomeTimelineExecuted(object sender, ExecutedRoutedEventArgs ea)
        {
            var statuses = (Status[])ea.Parameter;
            _timeline.Controller.UpdateStatus(statuses);
        }
    }
}