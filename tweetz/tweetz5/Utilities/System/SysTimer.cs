using System;
using System.Threading;
using System.Timers;
using Timer = System.Timers.Timer;

namespace tweetz5.Utilities.System
{
    public interface ITimer : IDisposable
    {
        void Start();
        double Interval { set; }
        event EventHandler Elapsed;
    }

    public sealed class SysTimer : ITimer
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
            set => _timer.Interval = value;
        }

        public event EventHandler Elapsed;

        private void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            try
            {
                Elapsed?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                // Timers eat exceptions. We don't
                ThreadPool.QueueUserWorkItem(_ => throw e);
            }
        }

        public static Func<ITimer> ImplementationOverride { private get; set; }

        public static ITimer Factory()
        {
            return (ImplementationOverride != null) ? ImplementationOverride() : new SysTimer();
        }

        private bool _disposed;

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            if (_timer == null) return;
            _timer.Stop();
            _timer.Elapsed -= TimerOnElapsed;
            _timer.Dispose();
            _timer = null;
        }
    }
}