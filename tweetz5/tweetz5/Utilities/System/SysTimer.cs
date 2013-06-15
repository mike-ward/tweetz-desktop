// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Timers;

namespace tweetz5.Utilities.System
{
    public interface ITimer : IDisposable
    {
        void Start();
        double Interval { get; set; }
        event EventHandler Elapsed;
    }

    public class SysTimer : ITimer
    {
        private Timer _timer = new Timer();

        private SysTimer()
        {
            _timer.Elapsed += TimerOnElapsed;
        }

        public void Start()
        {
            _timer.Start();
        }

        public double Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        public event EventHandler Elapsed;

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (Elapsed != null)
            {
                Elapsed(this, EventArgs.Empty);
            }
        }

        public static Func<ITimer> ImplementationOverride { get; set; }

        public static ITimer Factory()
        {
            return  (ImplementationOverride != null) ? ImplementationOverride() : new SysTimer();
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
                if (_timer != null)
                {
                    _timer.Stop();
                    _timer.Elapsed -= TimerOnElapsed;
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }
    }
}