// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Diagnostics;
using System.Media;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using tweetz5.Model;
using tweetz5.Properties;

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
        public static readonly RoutedCommand SignOutCommand = new RoutedUICommand();
        public static readonly RoutedCommand SettingsCommand = new RoutedUICommand();

        public MainWindow()
        {
            InitializeComponent();
            MainPanel.IsVisibleChanged += MainPanelOnIsVisibleChanged;
            Loaded += (sender, args) =>
            {
                // HACK: Compose.Toggle does not work the first time unless the control is initially visible.
                Compose.Visibility = Visibility.Collapsed;
                SignIn();
            };
        }

        public void SignIn()
        {
            SettingsPanel.Visibility = Visibility.Collapsed;
            if (string.IsNullOrWhiteSpace(Settings.Default.UserId))
            {
                AuthenticatePanel.Visibility = Visibility.Visible;
                MainPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                AuthenticatePanel.Visibility = Visibility.Collapsed;
                Timeline.Controller.StartTimelines();
                SwitchTimelinesCommand.Execute(Timelines.UnifiedName, this);
            }
        }

        private void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void ThumbOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Height = Math.Max(Height + e.VerticalChange, MinHeight);
        }

        private void MainPanelOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (MainPanel.IsVisible)
            {
                ResizeBar.Visibility = Visibility.Visible;
                Timeline.Visibility = Visibility.Visible;
                OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), false, true));
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Timeline.Height = e.NewSize.Height - Topbar.ActualHeight - NavBar.ActualHeight - Compose.ActualHeight - ResizeBar.ActualHeight;
        }

        private void ComposeOnClick(object sender, RoutedEventArgs e)
        {
            Compose.Toggle();
        }

        private void ComposeOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), false, true));
            if (Compose.IsVisible)
            {
                Compose.Focus();
            }
        }

        private void SwitchTimeslinesCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var timelineName = (string)ea.Parameter;
            SettingsPanel.Visibility = Visibility.Collapsed;
            ResizeBar.Visibility = Visibility.Visible;
            Timeline.Visibility = Visibility.Visible;
            MainPanel.Visibility = Visibility.Visible;
            SetButtonStates(timelineName);
            Timeline.Controller.SwitchTimeline(timelineName);
            Timeline.ScrollToTop();
            Timeline.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void SetButtonStates(string timelineName)
        {
            UnifiedButton.IsEnabled = timelineName != Timelines.UnifiedName;
            HomeButton.IsEnabled = timelineName != Timelines.HomeName;
            MentionsButton.IsEnabled = timelineName != Timelines.MentionsName;
            MessagesButton.IsEnabled = timelineName != Timelines.MessagesName;
            FavoritesButton.IsEnabled = timelineName != Timelines.FavoritesName;
            SearchButton.IsEnabled = timelineName != Timelines.SearchName;
            SettingsButton.IsEnabled = timelineName != "settings";
        }

        private void CopyTweetCommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = (Tweet)ea.Parameter;
            Clipboard.SetText(tweet.Text);
        }

        private void ReplyCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = (Tweet)ea.Parameter;
            var message = string.Format("@{0} ", tweet.ScreenName);
            Compose.Show(message, tweet.StatusId);
        }

        private void RetweetCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                ea.Handled = true;
                var tweet = (Tweet)ea.Parameter;
                if (tweet.IsRetweet)
                {
                    var id = string.IsNullOrWhiteSpace(tweet.RetweetStatusId) ? tweet.StatusId : tweet.RetweetStatusId;
                    var json = Twitter.GetTweet(id);
                    var status = Status.ParseJson("[" + json + "]")[0];
                    var retweetStatusId = status.CurrentUserRetweet.Id;
                    Twitter.DestroyStatus(retweetStatusId);
                    tweet.IsRetweet = false;
                }
                else
                {
                    Twitter.RetweetStatus(tweet.StatusId);
                    tweet.IsRetweet = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void RetweetClassicCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = (Tweet)ea.Parameter;
            var message = string.Format("RT @{0} {1}", tweet.ScreenName, tweet.Text);
            Compose.Show(message);
        }

        private void FavoritesCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                ea.Handled = true;
                var tweet = (Tweet)ea.Parameter;
                if (tweet.Favorited)
                {
                    Timeline.Controller.RemoveFavorite(tweet);
                }
                else
                {
                    Timeline.Controller.AddFavorite(tweet);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void UpdateStatusHomeTimelineHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var statuses = (Status[])ea.Parameter;
            Timeline.Controller.UpdateStatus(new[] {Timelines.HomeName, Timelines.UnifiedName}, statuses, "h");
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Close();
        }

        private void ShowUserInformationCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            UserInformationPopup.ScreenName = ea.Parameter as string;
            UserInformationPopup.IsOpen = true;
        }

        private void OpenLinkCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Process.Start(new ProcessStartInfo((string)ea.Parameter));
        }

        private void FollowCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var user = (User)ea.Parameter;
            user.Following = user.Following ? !Twitter.Unfollow(user.ScreenName) : Twitter.Follow(user.ScreenName);
        }

        private void NotifyCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var player = new SoundPlayer { Stream = Properties.Resources.Notify };
            player.Play();
        }

        private void DeleteTweetCommandHander(object sender, ExecutedRoutedEventArgs ea)
        {
            try
            {
                ea.Handled = true;
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
                ea.Handled = true;
                var query = ea.Parameter as string;
                if (string.IsNullOrWhiteSpace(query)) return;
                Timeline.SearchControl.SetSearchText(query);
                SwitchTimelinesCommand.Execute(Timelines.SearchName, this);
                Timeline.Controller.Search(query);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void AlertCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var message = ea.Parameter as string;
            if (string.IsNullOrWhiteSpace(message)) return;
            StatusAlert.Message.Text = message;
            StatusAlert.IsOpen = true;
        }

        private void SignOutCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Timeline.Controller.StopTimelines();
            Settings.Default.AccessToken = "";
            Settings.Default.AccessTokenSecret = "";
            Settings.Default.ScreenName = "";
            Settings.Default.UserId = "";
            Settings.Default.Save();
            SignIn();
        }

        private void SettingsCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            SettingsPanel.Visibility = Visibility.Visible;
            ResizeBar.Visibility = Visibility.Collapsed;
            Timeline.Visibility = Visibility.Collapsed;
            SetButtonStates("settings");
        }
    }
}