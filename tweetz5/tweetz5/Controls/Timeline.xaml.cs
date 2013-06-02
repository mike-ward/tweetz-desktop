// Copyright (c) 2013 Blue Onion Software - All rights reserved

using System.Windows;
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
    }
}