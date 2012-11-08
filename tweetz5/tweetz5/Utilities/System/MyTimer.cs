// Copyright (c) 2012 Blue Onion Software - All rights reserved

using System;
using System.Timers;

namespace tweetz5.Utilities.System
{
    public interface ITimer : IDisposable
    {
        double Interval { get; set; }
        event ElapsedEventHandler Elapsed;
        void Start();
    }

    public class MyTimer : ITimer
    {
        private Timer _timer = new Timer();

        private MyTimer()
        {
        }

        public static Func<ITimer> Override { get; set; }
        
        public static ITimer Factory()
        {
            return  (Override != null) ? Override() : new MyTimer();
        }

        public event ElapsedEventHandler Elapsed
        {
            add { _timer.Elapsed += value; }
            remove { _timer.Elapsed -= value; }
        }

        public double Interval
        {
            get { return _timer.Interval; }
            set { _timer.Interval = value; }
        }

        public void Start()
        {
            _timer.Start();
        }

        private bool _disposed;

        protected void Dispose(bool disposing)
        {
            if (_disposed) return;
            _disposed = true;
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}