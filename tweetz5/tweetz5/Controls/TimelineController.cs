// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using tweetz5.Model;
using tweetz5.Utilities.System;

namespace tweetz5.Controls
{
    public class TimelineController : IDisposable
    {
        private readonly ITimelines _timelinesModel;
        private ITimer _checkTimelines;
        private ITimer _updateTimeStamps;

        public TimelineController(ITimelines timelinesModel)
        {
            _timelinesModel = timelinesModel;
        }

        public void StartTimelines()
        {
            _checkTimelines = SysTimer.Factory();
            _checkTimelines.Interval = 100;
            _checkTimelines.Elapsed += (s, e) =>
            {
                _checkTimelines.Interval = 60000;
                _timelinesModel.HomeTimeline();
                _timelinesModel.MentionsTimeline();
            };
            _checkTimelines.Start();

            _updateTimeStamps = SysTimer.Factory();
            _updateTimeStamps.Interval = 30000;
            _updateTimeStamps.Elapsed += (s, e) => _timelinesModel.UpdateTimeStamps();
            _updateTimeStamps.Start();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                if (_checkTimelines != null)
                {
                    _checkTimelines.Dispose();
                    _checkTimelines = null;
                }
                if (_updateTimeStamps != null)
                {
                    _updateTimeStamps.Dispose();
                    _updateTimeStamps = null;
                }
            }
        }
    }
}