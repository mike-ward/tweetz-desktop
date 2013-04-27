// Copyright (c) 2012 Blue Onion Software. All rights reserved.

using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace tweetz5
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
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
            var composeHeight = _compose.Visibility == Visibility.Visible ? _compose.ActualHeight : 0;
            _timeline.Height = e.NewSize.Height - composeHeight - 20;
        }

        private void ComposeOnClick(object sender, RoutedEventArgs e)
        {
            _compose.Visibility = (_compose.Visibility == Visibility.Hidden) 
                ? Visibility.Visible 
                : Visibility.Hidden;

            var sizeChangedInfo = new SizeChangedInfo(this, new Size(Width, Height), false, true);
            OnRenderSizeChanged(sizeChangedInfo);
            _compose.Focus();
        }
    }
}