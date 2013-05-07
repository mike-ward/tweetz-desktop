// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Windows;
using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public partial class Timeline
    {
        private readonly TimelineController _controller;

        public Timeline()
        {
            InitializeComponent();
            _controller = new TimelineController((Timelines) DataContext);
            _controller.StartTimelines();
            Unloaded += (sender, args) => _controller.Dispose();
        }

        public void CopyTweetCommand(object target, ExecutedRoutedEventArgs ex)
        {
            var tweet = (Tweet)ex.Parameter;
            Clipboard.SetText(tweet.Text);
        }

        private void CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }
}