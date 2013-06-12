// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
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
        public static readonly RoutedCommand DeleteTweetCommand = new RoutedUICommand();
        public static readonly RoutedCommand CopyCommand = new RoutedUICommand();
        public static readonly RoutedCommand UpdateStatusHomeTimelineCommand = new RoutedUICommand();
        public static readonly RoutedCommand SwitchTimelinesCommand = new RoutedUICommand();
        public static readonly RoutedCommand ShowUserInformationCommand = new RoutedUICommand();
        public static readonly RoutedCommand OpenLinkCommand = new RoutedUICommand();
        public static readonly RoutedCommand FollowUserComand = new RoutedUICommand();
        public static readonly RoutedCommand SearchCommand = new RoutedUICommand();
        public static readonly RoutedCommand AlertCommand = new RoutedUICommand();

        public MainWindow()
        {
            InitializeComponent();
            // HACK: Compose.Toggle does not work the first time unless the control is initially visible.
            Loaded += (sender, args) => Compose.Visibility = Visibility.Collapsed;
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
            Timeline.Height = e.NewSize.Height - Topbar.ActualHeight - NavBar.ActualHeight - Compose.ActualHeight - ResizeBar.ActualHeight;
        }

        private void ComposeOnClick(object sender, RoutedEventArgs e)
        {
            Compose.Toggle();
        }

        private void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), false, true));
            if (Compose.IsVisible)
            {
                Compose.Focus();
            }
        }

        private void SwitchTimeslinesCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var timeline = (string)ea.Parameter;
            SetButtonStates(timeline);
            Timeline.Controller.SwitchTimeline(timeline);
            Timeline.ScrollToTop();
            Timeline.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void SetButtonStates(string timeline)
        {
            UnifiedButton.IsEnabled = timeline != "unified";
            HomeButton.IsEnabled = timeline != "home";
            MentionsButton.IsEnabled = timeline != "mentions";
            MessagesButton.IsEnabled = timeline != "messages";
            FavoritesButton.IsEnabled = timeline != "favorites";
            SearchButton.IsEnabled = timeline != "search";
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
            Compose.Show(message, tweet.StatusId);
        }

        private void RetweetCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                string json;
                var tweet = (Tweet)ea.Parameter;
                if (tweet.IsRetweet)
                {
                    json = Twitter.GetTweet(tweet.StatusId);
                    var status = Status.ParseJson("[" + json + "]")[0];
                    var retweetStatusId = status.CurrentUserRetweet.Id;
                    json = Twitter.DestroyStatus(retweetStatusId);
                }
                else
                {
                    json = Twitter.RetweetStatus(tweet.StatusId);
                }
                if (json.Contains(tweet.StatusId))
                {
                    var status = Status.ParseJson("[" + json + "]")[0];
                    tweet.RetweetedBy = Timelines.RetweetedBy(status);
                    tweet.IsRetweet = status.Retweeted;
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
            Compose.Show(message);
        }

        private void FavoritesCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                var tweet = (Tweet)ea.Parameter;
                var json = tweet.Favorited ? Twitter.DestroyFavorite(tweet.StatusId) : Twitter.CreateFavorite(tweet.StatusId);
                if (json.Contains(tweet.StatusId)) tweet.Favorited = !tweet.Favorited;
                if (tweet.Favorited)
                {
                    var status = Status.ParseJson("[" + json + "]");
                    Timeline.Controller.UpdateStatus(new[] { "favorites" }, status, "f");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UpdateStatusHomeTimelineHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var statuses = (Status[])ea.Parameter;
            Timeline.Controller.UpdateStatus(new[] { "home", "unified" }, statuses, "h");
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void ShowUserInformationCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            UserInformationPopup.ScreenName = ea.Parameter as string;
            UserInformationPopup.IsOpen = true;
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
            var player = new SoundPlayer { Stream = Properties.Resources.Notify };
            player.Play();
        }

        private void DeleteTweetCommandHander(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                var tweet = (Tweet)ea.Parameter;
                Twitter.DestroyStatus(tweet.StatusId);
                Timeline.Controller.RemoveStatus(tweet);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void SearchCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                var query = ea.Parameter as string;
                if (string.IsNullOrWhiteSpace(query)) return;
                Timeline.SearchControl.SetSearchText(query);
                Timeline.Controller.ClearSearchTimeline();
                SwitchTimelinesCommand.Execute("search", this);
                Task.Run(() =>
                {
                    var json = Twitter.Search(query);
                    var statuses = SearchStatuses.ParseJson(json);
                    Timeline.Controller.UpdateStatus(new[] { "search" }, statuses, string.Empty);
                });
                ea.Handled = true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AlertCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            var message = ea.Parameter as string;
            if (string.IsNullOrWhiteSpace(message)) return;
            StatusAlert.Message.Text = message;
            StatusAlert.IsOpen = true;
            ea.Handled = true;
        }
    }
}