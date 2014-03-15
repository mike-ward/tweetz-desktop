using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using tweetz5.Commands;
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
                ApplyCommandBindings();
                SizeChanged += OnSizeChanged;
                Drop += DragAndDrop.OnDrop;
                DragOver += DragAndDrop.OnDragOver;
                MouseLeftButtonDown += DragMoveWindow;
                ChangeTheme.Command.Execute(Settings.Default.Theme, this);
                SetFontSizeCommand.Command.Execute(Settings.Default.FontSize, this);

                // WPF HACK: Compose.Toggle does not work the first time unless the control is initially visible.
                Compose.Visibility = Visibility.Collapsed;

                // ReSharper disable once PossibleNullReferenceException
                HwndSource.FromHwnd(new WindowInteropHelper(this).Handle).AddHook(WndProc);

                if (BuildInfo.HasExpired()) return;
                SignInCommand.Command.Execute(null, this);
            };
        }

        private void ApplyCommandBindings()
        {
            CommandBindings.Add(new CommandBinding(ChangeTheme.Command, ChangeTheme.CommandHandler));
            CommandBindings.Add(new CommandBinding(SetFontSizeCommand.Command, SetFontSizeCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(SignInCommand.Command, SignInCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(ReplyCommand.Command, ReplyCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(RetweetCommand.Command, RetweetCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(RetweetClassicCommand.Command, RetweetClassicCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(FavoritesCommand.Command, FavoritesCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(DeleteTweetCommand.Command, DeleteTweetCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(CopyCommand.Command, CopyCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(CopyLinkCommand.Command, CopyLinkCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(OpenTweetLinkCommand.Command, OpenTweetLinkCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(UpdateStatusHomeTimelineCommand.Command, UpdateStatusHomeTimelineCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(SwitchTimelinesCommand.Command, SwitchTimelinesCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(ShowUserInformationCommand.Command, ShowUserInformationCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(OpenLinkCommand.Command, OpenLinkCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(FollowUserCommand.Command, FollowUserCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(SearchCommand.Command, SearchCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(AlertCommand.Command, AlertCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(SignOutCommand.Command, SignOutCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(SettingsCommand.Command, SettingsCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(UpdateLayoutCommand.Command, UpdateLayoutCommandHandler));
            CommandBindings.Add(new CommandBinding(OpenComposeCommand.Command, OpenComposeCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(ShortcutHelpCommand.Command, ShortcutHelpCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(ChirpCommand.Command, ChirpCommand.CommandHandler));
            CommandBindings.Add(new CommandBinding(RestartTimelinesCommand.Command, RestartTimelinesCommand.CommandHandler));
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

        private void TopBarOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), true, true));
        }

        private void NavBarOnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), true, true));
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

        private void UpdateLayoutCommandHandler(object sender, ExecutedRoutedEventArgs ea)
        {
            ea.Handled = true;
            OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), true, true));
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable UnusedMember.Global
        // ReSharper disable UnusedField.Compiler
        // ReSharper disable once MemberCanBePrivate.Global
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
        // ReSharper restore UnusedField.Compiler
        // ReSharper restore UnusedMember.Global

        private static IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                // Allow window to move above top of screen
                // http://stackoverflow.com/questions/328127/how-do-i-move-a-wpf-window-into-a-negative-top-value
                case 0x46: //WM_WINDOWPOSCHANGING
                    if (Mouse.LeftButton != MouseButtonState.Pressed)
                    {
                        var wp = (WINDOWPOS)Marshal.PtrToStructure(lParam, typeof(WINDOWPOS));
                        wp.flags = wp.flags | 2; //SWP_NOMOVE
                        Marshal.StructureToPtr(wp, lParam, false);
                    }
                    break;
            }
            return IntPtr.Zero;
        }
    }
}