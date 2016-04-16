using System;

namespace Rabbitual
{
    public class Timer
    {
        private  System.Timers.Timer _timer;
        private  int _errCount;

        public void Stop()
        {
            _timer.Stop();
        }

        public void Start(int intervalMs, Action action)
        {
            Action<string> c = (s) => System.Console.WriteLine(s);

            _timer = new System.Timers.Timer();
            _timer.Elapsed += (a, b) =>
            {
                _timer.Stop();

                if(intervalMs>4000)
                    c("Scheduled task started");

                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    c("Error");
                    c(ex.Message);
                    c(ex.StackTrace);
                    if (_errCount++ > 10)
                    {
                        c("Aborting. Too many errors.");
                        return;
                    }
                }

                if (intervalMs > 4000)
                    c("Scheduled task done");


                _timer.Start(); //TODO: Will restart stopped task. Bad for TopShelf
            };
            _timer.Interval = intervalMs;
            _timer.Enabled = true;
        }
    }
}