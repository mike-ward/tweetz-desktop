using System;
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
using tweetz5.Model;
using tweetz5.Utilities.System;
using Settings = tweetz5.Properties.Settings;

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
                CommandBindings.Add(new CommandBinding(Commands.ChangeTheme.Command, Commands.ChangeTheme.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.SetFontSizeCommand.Command, Commands.SetFontSizeCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.SignInCommand.Command, Commands.SignInCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.ReplyCommand.Command, Commands.ReplyCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.RetweetCommand.Command, Commands.RetweetCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.RetweetClassicCommand.Command, Commands.RetweetClassicCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.FavoritesCommand.Command, Commands.FavoritesCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.DeleteTweetCommand.Command, Commands.DeleteTweetCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.CopyCommand.Command, Commands.CopyCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.CopyLinkCommand.Command, Commands.CopyLinkCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.OpenTweetLinkCommand.Command, Commands.OpenTweetLinkCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.UpdateStatusHomeTimelineCommand.Command, Commands.UpdateStatusHomeTimelineCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.SwitchTimelinesCommand.Command, Commands.SwitchTimelinesCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.ShowUserInformationCommand.Command, Commands.ShowUserInformationCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.OpenLinkCommand.Command, Commands.OpenLinkCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.FollowUserCommand.Command, Commands.FollowUserCommand.CommandHandler));
                CommandBindings.Add(new CommandBinding(Commands.SearchCommand.Command, Commands.SearchCommand.CommandHandler));

                Commands.ChangeTheme.Command.Execute(Settings.Default.Theme, this);
                Commands.SetFontSizeCommand.Command.Execute(Settings.Default.FontSize, this);

                // WPF HACK: Compose.Toggle does not work the first time unless the control is initially visible.
                Compose.Visibility = Visibility.Collapsed;

                // ReSharper disable once PossibleNullReferenceException
                HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(WndProc);

                if (HasExpired() == false) Commands.SignInCommand.Command.Execute(null, this);
            };
        }

        private bool HasExpired()
        {
            var buildDate = BuildInfo.GetBuildDateTime();
            if (DateTime.Now > buildDate.AddMonths(3))
            {
                Settings.Default.AccessToken = string.Empty;
                Settings.Default.AccessTokenSecret = string.Empty;
                MyCommands.AlertCommand.Execute("Expired", this);
                return true;
            }
            return false;
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


        internal void SetButtonStates(string timelineName)
        {
            UnifiedButton.IsEnabled = timelineName != Timelines.UnifiedName;
            HomeButton.IsEnabled = timelineName != Timelines.HomeName;
            MentionsButton.IsEnabled = timelineName != Timelines.MentionsName;
            MessagesButton.IsEnabled = timelineName != Timelines.MessagesName;
            FavoritesButton.IsEnabled = timelineName != Timelines.FavoritesName;
            SearchButton.IsEnabled = timelineName != Timelines.SearchName;
            SettingsButton.IsEnabled = timelineName != "settings";
        }

        private void CloseCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            Close();
        }

        private void NotifyCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            if (Settings.Default.Chirp == false) return;
            var player = new SoundPlayer {Stream = Properties.Resources.Notify};
            player.Play();
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
            Commands.SignInCommand.Command.Execute(null, this) ;
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