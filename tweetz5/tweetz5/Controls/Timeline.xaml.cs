// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System;
using System.Windows;
using System.Windows.Input;
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
            Controller.StartTimelines();
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
            try
            {
                TimelineItems.ScrollIntoView(TimelineItems.Items[0]);
            }
            catch (ArgumentOutOfRangeException)
            {
            }
        }
    }
}