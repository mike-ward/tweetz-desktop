// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using tweetz5.Controls;
using tweetz5.Model;
using tweetz5.Utilities.System;
using Settings = tweetz5.Properties.Settings;

// ReSharper disable CanBeReplacedWithTryCastAndCheckForNull

namespace tweetz5
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            MainPanel.IsVisibleChanged += MainPanelOnIsVisibleChanged;
            Loaded += (sender, args) =>
            {
                var buildDate = BuildInfo.GetBuildDateTime();
                if (DateTime.Now > buildDate.AddMonths(3))
                {
                    Settings.Default.AccessToken = string.Empty;
                    Settings.Default.AccessTokenSecret = string.Empty;
                    MyCommands.AlertCommand.Execute("Expired", this);
                    return;
                }
                // HACK: Compose.Toggle does not work the first time unless the control is initially visible.
                Compose.Visibility = Visibility.Collapsed;
                MyCommands.SetFontSizeCommand.Execute(Settings.Default.FontSize, this);
                MyCommands.SignInCommand.Execute(null, this);

                // ReSharper disable once PossibleNullReferenceException
                HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(WndProc);

                CommandBindings.Add(new CommandBinding(Commands.ChangeTheme.Command, Commands.ChangeTheme.CommandHandler));
                Commands.ChangeTheme.Command.Execute(Settings.Default.Theme, this);
            };
        }

        private void SignInCommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
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
                Compose.Friends = Timeline.Controller.ScreenNames;
            }
        }

        private void DragMoveWindow(object sender, MouseButtonEventArgs e)
        {
            DragMove();
            e.Handled = true;
        }

        private void TopSizeOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var oldHeight = Height;
            Height = Math.Max(Height - Screen.VerticalPixelToDpi(this, e.VerticalChange), MinHeight);
            Top += Screen.VerticalDpiToPixel(this, oldHeight - Height);
        }

        private void BottomSizeOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Height = Math.Max(Height + Screen.VerticalPixelToDpi(this, e.VerticalChange), MinHeight);
        }

        private void RightSizeBarOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            Width = Math.Max(Width + Screen.HorizontalPixelToDpi(this, e.HorizontalChange), MinWidth);
        }

        private void LeftSizeBarOnDragDelta(object sender, DragDeltaEventArgs e)
        {
            var oldWidth = Width;
            Width = Math.Max(Width - Screen.HorizontalPixelToDpi(this, e.HorizontalChange), MinWidth);
            Left += Screen.HorizontalDpiToPixel(this, oldWidth - Width);
        }

        private void MainPanelOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (MainPanel.IsVisible)
            {
                BottomResizeBar.Visibility = Visibility.Visible;
                Timeline.Visibility = Visibility.Visible;
                OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), true, true));
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            Timeline.Height = Math.Max(0, 
                e.NewSize.Height 
                - TopResizeBar.ActualHeight 
                - TopBarSpacer.ActualHeight
                - NavBarSpacer.ActualHeight
                - Topbar.ActualHeight 
                - NavBar.ActualHeight 
                - Compose.ActualHeight 
                - BottomResizeBar.ActualHeight);
            Timeline.Width = Math.Max(0, e.NewSize.Width - LeftSizeBar.ActualWidth - RightSizeBar.ActualWidth);

            SettingsPanel.Height = Timeline.Height;
            SettingsPanel.Width = Timeline.Width;

            AuthenticatePanel.Width = Timeline.Width;
            Compose.Width = Timeline.Width;
        }

        private void ComposeOnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), true, true));
            if (Compose.IsVisible) Compose.Focus();
        }

        private void SwitchTimeslinesCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var timelineName = (string) ea.Parameter;
            Compose.Visibility = Visibility.Collapsed;
            SettingsPanel.Visibility = Visibility.Collapsed;
            BottomResizeBar.Visibility = Visibility.Visible;
            Timeline.Visibility = Visibility.Visible;
            MainPanel.Visibility = Visibility.Visible;
            SetButtonStates(timelineName);
            Timeline.Controller.SwitchTimeline(timelineName);
            Timeline.ScrollToTop();
            Timeline.Focus();
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

        private void CopyCommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = ea.Parameter as Tweet ?? Timeline.GetSelectedTweet;
            if (tweet == null) return;
            Timeline.Controller.CopyTweetToClipboard(tweet);
        }

        private void CopyLinkCommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = ea.Parameter as Tweet ?? Timeline.GetSelectedTweet;
            if (tweet == null) return;
            Timeline.Controller.CopyLinkToClipboard(tweet);
        }

        private void OpenTweetLinkCommandHandler(object target, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = (Tweet) ea.Parameter ?? Timeline.GetSelectedTweet;
            var link = TimelineController.TweetLink(tweet);
            MyCommands.OpenLinkCommand.Execute(link, this);
        }

        private void ReplyCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = ea.Parameter as Tweet ?? Timeline.GetSelectedTweet;
            if (tweet == null) return;
            if (tweet.IsDirectMesssage)
            {
                Compose.ShowDirectMessage(tweet.Name, tweet.ScreenName);
            }
            else
            {
                var matches = new Regex(@"(?<=^|(?<=[^a-zA-Z0-9-_\.]))@([A-Za-z]+[A-Za-z0-9_]+)")
                    .Matches(tweet.Text);

                var names = matches
                    .Cast<Match>()
                    .Where(m => m.Groups[1].Value != tweet.ScreenName)
                    .Where(m => m.Groups[1].Value != Settings.Default.ScreenName)
                    .Select(m => "@" + m.Groups[1].Value)
                    .Distinct();

                var replyTos = string.Join(" ", names);
                var message = string.Format("@{0} {1}{2}", tweet.ScreenName, replyTos, (replyTos.Length > 0) ? " " : "");
                Compose.Show(message, tweet.StatusId);
            }
        }

        private void RetweetCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = ea.Parameter as Tweet ?? Timeline.GetSelectedTweet;
            if (tweet == null) return;
            Timeline.Controller.Retweet(tweet);
        }

        private void RetweetClassicCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = (Tweet) ea.Parameter ?? Timeline.GetSelectedTweet;
            var message = string.Format("RT @{0} {1}", tweet.ScreenName, tweet.Text);
            Compose.Show(message);
        }

        private void FavoritesCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = ea.Parameter as Tweet ?? Timeline.GetSelectedTweet;
            if (tweet == null) return;
            if (tweet.Favorited) Timeline.Controller.RemoveFavorite(tweet);
            else Timeline.Controller.AddFavorite(tweet);
        }

        private void UpdateStatusHomeTimelineHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var statuses = (Status[]) ea.Parameter;
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
            Process.Start(new ProcessStartInfo((string) ea.Parameter));
        }

        private void FollowCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var user = (User) ea.Parameter;
            user.Following = user.Following ? !Twitter.Unfollow(user.ScreenName) : Twitter.Follow(user.ScreenName);
        }

        private void NotifyCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            if (Settings.Default.Chirp == false) return;
            var player = new SoundPlayer {Stream = Properties.Resources.Notify};
            player.Play();
        }

        private void DeleteTweetCommandHander(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var tweet = ea.Parameter as Tweet;
            if (tweet == null) return;
            Timeline.Controller.DeleteTweet(tweet);
        }

        private void SearchCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var query = ea.Parameter as string;
            if (string.IsNullOrWhiteSpace(query)) return;
            Timeline.SearchControl.SetSearchText(query);
            MyCommands.SwitchTimelinesCommand.Execute(Timelines.SearchName, this);
            Timeline.Controller.Search(query);
        }

        private void AlertCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var message = ea.Parameter as string;
            if (string.IsNullOrWhiteSpace(message)) return;
            StatusAlert.Message.Text = message;
            StatusAlert.IsOpen = true;
        }

        private void ShortcutHelpCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            ShortcutHelp.IsOpen = true;
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
            MyCommands.SignInCommand.Execute(null, this);
        }

        private void SettingsCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            SettingsPanel.Visibility = Visibility.Visible;
            Timeline.Visibility = Visibility.Collapsed;
            SetButtonStates("settings");
        }

        private void OpenComposeCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Compose.Toggle();
        }

        private void UpdateLayoutCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), true, true));
        }

        private void OnDragOver(object sender, DragEventArgs ea)
        {
            ea.Handled = true;
            ea.Effects = DragDropEffects.None;
            if (ea.Data.GetDataPresent("text/html"))
            {
                ea.Effects = DragDropEffects.Copy;
            }
            else if (ea.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var filenames = ea.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames != null && filenames.Length == 1 && IsValidImageExtension(filenames[0]))
                {
                    ea.Effects = DragDropEffects.Copy;
                }
            }
        }

        private void OnDrop(object sender, DragEventArgs ea)
        {
            if (ea.Data.GetDataPresent("text/html"))
            {
                var html = string.Empty;
                var data = ea.Data.GetData("text/html");
                if (data is string)
                {
                    html = (string) data;
                }
                else if (data is MemoryStream)
                {
                    var stream = (MemoryStream) data;
                    var buffer = new byte[stream.Length];
                    stream.Read(buffer, 0, buffer.Length);
                    html = (buffer[1] == (byte) 0)
                        ? Encoding.Unicode.GetString(buffer)
                        : Encoding.ASCII.GetString(buffer);
                }
                var match = new Regex(@"<img[^>]+src=""([^""]*)""").Match(html);
                if (match.Success)
                {
                    var uri = new Uri(match.Groups[1].Value);
                    var filename = Path.GetTempFileName();
                    var webClient = new WebClient();
                    try
                    {
                        webClient.DownloadFileCompleted += (o, args) =>
                        {
                            Compose.Visibility = Visibility.Visible;
                            Compose.Image = filename;
                            webClient.Dispose();
                        };
                        webClient.DownloadFileAsync(uri, filename);
                        ea.Handled = true;
                    }
                    catch (WebException)
                    {
                    }
                }
            }
            else if (ea.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                var filenames = ea.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (filenames != null && filenames.Length == 1 && IsValidImageExtension(filenames[0]))
                {
                    Compose.Visibility = Visibility.Visible;
                    Compose.Image = filenames[0];
                    ea.Handled = true;
                }
            }
        }

        private static bool IsValidImageExtension(string filename)
        {
            var extension = Path.GetExtension(filename) ?? string.Empty;
            var extensions = new[] {".png", ".jpg", ".jpeg", ".gif"};
            return extensions.Any(e => extension.Equals(e, StringComparison.OrdinalIgnoreCase));
        }

        private void SetFontSizeCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            var size = double.Parse(ea.Parameter.ToString());
            Application.Current.Resources["AppFontSize"] = size;
            Application.Current.Resources["AppFontSizePlus1"] = size + 1;
            Application.Current.Resources["AppFontSizePlus3"] = size + 3;
            Application.Current.Resources["AppFontSizePlus7"] = size + 7;
            Application.Current.Resources["AppFontSizeMinus1"] = size - 1;
        }

        // ReSharper disable InconsistentNaming
        public struct WINDOWPOS
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
            public IntPtr hwnd;
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2111:PointersShouldNotBeVisible")]
            public IntPtr hwndInsertAfter;
            public int x;
            public int y;
            public int cx;
            public int cy;
            public UInt32 flags;
        };

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                // Allow window to move above top of screen
                // http://stackoverflow.com/questions/328127/how-do-i-move-a-wpf-window-into-a-negative-top-value
                case 0x46: //WM_WINDOWPOSCHANGING
                    if (Mouse.LeftButton != MouseButtonState.Pressed)
                    {
                        var wp = (WINDOWPOS) Marshal.PtrToStructure(lParam, typeof (WINDOWPOS));
                        wp.flags = wp.flags | 2; //SWP_NOMOVE
                        Marshal.StructureToPtr(wp, lParam, false);
                    }
                    break;
            }
            return IntPtr.Zero;
        }

        private void TopBarOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), true, true));
        }
    }
}