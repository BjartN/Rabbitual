using System;
using Rabbitual.Core.Logging;
using Rabbitual.Logging;

namespace Rabbitual.Infrastructure
{
    public class Timer
    {
        private readonly ILogger _log;
        private  System.Timers.Timer _timer;
        private  int _errCount;

        public Timer(ILogger log)
        {
            _log = log;
        }

        public void Stop()
        {
            _timer.Stop();
        }

        public void Start(int intervalMs, Action action)
        {
            _timer = new System.Timers.Timer();
            _timer.Elapsed += (a, b) =>
            {
                _timer.Stop();

                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    _log.Warn("Error");
                    _log.Warn(ex.Message);
                    _log.Warn(ex.StackTrace);
                    if (_errCount++ > 10)
                    {
                        _log.Warn("Aborting. Too many errors.");
                        return;
                    }
                }

                _timer.Start(); //TODO: Will restart stopped task. Bad for TopShelf
            };
            _timer.Interval = intervalMs;
            _timer.Enabled = true;
        }
    }
}