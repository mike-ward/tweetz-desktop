// Copyright (c) 2012 Blue Onion Software - All rights reserved

using tweetz5.Model;
using tweetz5.Utilities.System;

namespace tweetz5
{
    public class TimelineController
    {
        private readonly ITimelines _timelinesModel;
        private ITimer _checkTimelines;
        private ITimer _updateTimeStamps;

        public TimelineController(ITimelines timelinesModel)
        {
            _timelinesModel = timelinesModel;
            StartTimelines();
        }

        private void StartTimelines()
        {
            _checkTimelines = MyTimer.Factory();
            _checkTimelines.Interval = 100;
            _checkTimelines.Elapsed += (s, e) =>
            {
                _checkTimelines.Interval = 60000;
                _timelinesModel.HomeTimeline();
                _timelinesModel.MentionsTimeline();
            };
            _checkTimelines.Start();

            _updateTimeStamps = MyTimer.Factory();
            _updateTimeStamps.Interval = 30000;
            _updateTimeStamps.Elapsed += (s, e) => _timelinesModel.UpdateTimeStamps();
            _updateTimeStamps.Start();
        }
    }
}