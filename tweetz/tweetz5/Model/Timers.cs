using System;
using System.Collections.Generic;
using tweetz5.Utilities.System;

namespace tweetz5.Model
{
    public sealed class Timers : IDisposable
    {
        private bool _disposed;
        private List<ITimer> _timers = new List<ITimer>();

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _timers.ForEach(timer => timer.Dispose());
            _timers.Clear();
            _timers = null;
        }

        public void Add(int seconds, EventHandler handler)
        {
            if (_disposed) throw new ObjectDisposedException(string.Empty);
            var timer = SysTimer.Factory();
            timer.Interval = 100;
            timer.Elapsed += (s, e) =>
            {
                timer.Interval = seconds*1000;
                handler(s, e);
            };
            timer.Start();
        }
    }
}