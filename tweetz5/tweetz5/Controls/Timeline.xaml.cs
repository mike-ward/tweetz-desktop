// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Windows.Input;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public partial class Timeline
    {
        private readonly TimelineController _controller;
        public static readonly RoutedCommand ReplyCommand = new RoutedUICommand();
        public static readonly RoutedCommand RetweetCommand = new RoutedUICommand();
        public static readonly RoutedCommand RetweetClassicCommand = new RoutedUICommand();
        public static readonly RoutedCommand FavoritesCommand = new RoutedUICommand();
        public static readonly RoutedCommand CopyCommand = new RoutedUICommand();

        public Timeline()
        {
            InitializeComponent();
            _controller = new TimelineController((Timelines)DataContext);
            _controller.StartTimelines();
            Unloaded += (sender, args) => _controller.Dispose();
        }

        private void CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CopyTweetCommand(object target, ExecutedRoutedEventArgs ea)
        {
            var tweet = (Tweet)ea.Parameter;
            _controller.CopyTweetToClipboard(tweet);
        }

        private void ReplyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void RetweetCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void RetweetClassicCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        private void FavoritesCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
        }

        public void UpdateStatus(Status[] statuses)
        {
            _controller.UpdateStatus(statuses);
        }
    }
}