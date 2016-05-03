using System;

namespace Rabbitual.Logging
{
    public interface ILogger
    {
        void Info(string s, params object[] args);
        void Warn(string s, params object[] args);
        void Debug(string s, params object[] args);

    }

    public class Logger: ILogger
    {
        public void Info(string s, params object[] args)
        {
            Console.WriteLine(s, args);
        }

        public void Warn(string s, params object[] args)
        {
            Console.WriteLine(s, args);
        }

        public void Debug(string s, params object[] args)
        {
            Console.WriteLine(s, args);
        }
    }

}