using System;
using System.Diagnostics;

namespace Firk.Core.Actions
{
    public abstract class BackgroundIntervalAction : BackgroundAction
    {
        private readonly Stopwatch _stopwatch;

        public BackgroundIntervalAction()
        {
            // デフォルトは10min
            Interval = TimeSpan.FromMinutes(10);
            _stopwatch = new Stopwatch();
            _stopwatch.Start();
        }

        protected BackgroundIntervalAction(TimeSpan interval) : this()
        {
            Interval = interval;
        }

        protected TimeSpan Interval { get; private set; }
        protected abstract void InvokeCore();

        protected override void InvokeCoreAsync()
        {
            if (_stopwatch.Elapsed.CompareTo(Interval) > 0)
            {
                InvokeCore();
                _stopwatch.Restart();
            }
        }
    }
}