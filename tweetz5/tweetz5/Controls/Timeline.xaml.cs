// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using tweetz5.Model;

namespace tweetz5.Controls
{
    public partial class Timeline
    {
        public TimelineController Controller { get; private set; }

        public Timeline()
        {
            InitializeComponent();
            Controller = new TimelineController((Timelines)DataContext);
            TimelineItems.PreviewMouseWheel += TimelineItemsOnPreviewMouseWheel;
            Unloaded += (sender, args) => Controller.Dispose();
        }

        private void MoreOnMouseDown(object sender, MouseButtonEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            frameworkElement.ContextMenu.PlacementTarget = this;
            frameworkElement.ContextMenu.DataContext = frameworkElement.DataContext;
            frameworkElement.ContextMenu.IsOpen = true;
        }

        public void ScrollToTop()
        {
            if (TimelineItems.Items.Count > 0) TimelineItems.ScrollIntoView(TimelineItems.Items[0]);
        }

        private static void TimelineItemsOnPreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Modify listbox to scroll one line at a time.
            var scrollHost = (DependencyObject)sender;
            var scrollViewer = (ScrollViewer)GetScrollViewer(scrollHost);
            var offset = scrollViewer.VerticalOffset - (e.Delta > 0 ? 1 : -1);
            if (offset < 0)
            {
                scrollViewer.ScrollToVerticalOffset(0);
            }
            else if (offset > scrollViewer.ExtentHeight)
            {
                scrollViewer.ScrollToVerticalOffset(scrollViewer.ExtentHeight);
            }
            else
            {
                scrollViewer.ScrollToVerticalOffset(offset);
            }
            e.Handled = true;
        }

        public static DependencyObject GetScrollViewer(DependencyObject o)
        {
            if (o is ScrollViewer)
            {
                return o;
            }
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(o); i++)
            {
                var child = VisualTreeHelper.GetChild(o, i);
                var result = GetScrollViewer(child);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        public Tweet GetSelectedTweet
        {
            get { return TimelineItems.SelectedItem as Tweet; }
        }
    }
}