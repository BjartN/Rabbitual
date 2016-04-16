using System;

namespace Rabbitual.Infrastructure
{
    public interface ILogger
    {
        void Log(string s, params object[] args);
    }

    public class Logger: ILogger
    {
        public void Log(string s, params object[] args)
        {
            Console.WriteLine(s, args);
        }
    }

}