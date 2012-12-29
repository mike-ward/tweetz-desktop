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
            _timeline.Height = e.NewSize.Height - 20;
        }
    }
}