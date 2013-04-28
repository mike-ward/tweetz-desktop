// Copyright (c) 2012 Blue Onion Software. All rights reserved.

using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace tweetz5
{
    public partial class MainWindow
    {
        readonly DispatcherTimer _wpfClusterFuckTimer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            _wpfClusterFuckTimer.Interval = new TimeSpan(1);
            _wpfClusterFuckTimer.Tick += (o, args) =>
            {
                _wpfClusterFuckTimer.Stop();
                OnRenderSizeChanged(new SizeChangedInfo(this, new Size(Width, Height), false, true));
                _compose.Focus();
            };
        }

        private void DragMoveWindow(object sender, System.Windows.Input.MouseButtonEventArgs e)
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
            _compose.Visibility = _compose.IsVisible ? Visibility.Collapsed : Visibility.Visible;
            _wpfClusterFuckTimer.Start();
        }
    }
}