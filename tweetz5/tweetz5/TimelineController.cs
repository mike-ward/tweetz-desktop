// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System.Timers;
using tweetz5.Model;

namespace tweetz5
{
    public class TimelineController
    {
        private readonly Timelines _timelinesModel;
        private Timer _checkTimelines;
        private Timer _updateTimeStamps;

        public TimelineController(Timelines timelinesModel)
        {
            _timelinesModel = timelinesModel;
            StartTimelines();
        }

        private void StartTimelines()
        {
            _checkTimelines = new Timer(100);
            _checkTimelines.Elapsed += (s, e) =>
            {
                _checkTimelines.Interval = 60000;
                _timelinesModel.HomeTimeline();
                _timelinesModel.MentionsTimeline();

            };
            _checkTimelines.Start();

            _updateTimeStamps = new Timer(30000);
            _updateTimeStamps.Elapsed += (s, e) => _timelinesModel.UpdateTimeStamps();
            _updateTimeStamps.Start();
        }
    }
}