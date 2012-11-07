// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using tweetz5.Model;

namespace tweetz5
{
    // ReSharper disable NotAccessedField.Local

    public partial class Timeline
    {
        private readonly TimelineController _controller;

        public Timeline()
        {
            InitializeComponent();
            _controller = new TimelineController((Timelines) DataContext);
        }
    }
}