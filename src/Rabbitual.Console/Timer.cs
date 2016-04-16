using System;
using System.Timers;

namespace Rabbitual.Console
{
    public class Timer
    {

        private static System.Timers.Timer _timer;
        private static int _errCount = 0;
        private static int _counter = 0;

        public static void DoOnTimer(Action doIt)
        {
            Action<string> c = (s) => System.Console.WriteLine(s);

            _timer = new System.Timers.Timer();
            _timer.Elapsed += (a, b) =>
            {
                _timer.Stop();

                c("Starting..");

                try
                {
                    doIt();
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
                c("Done..");


                _timer.Start(); //TODO: Will restart stopped task. Bad for TopShelf
            };
            _timer.Interval = 5000;
            _timer.Enabled = true;
        }
    }
}