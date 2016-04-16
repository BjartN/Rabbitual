using System;

namespace Rabbitual
{
    public interface ILog
    {
        void Log(string s, params object[] args);
    }

    public class LogImpl: ILog
    {
        public void Log(string s, params object[] args)
        {
            Console.WriteLine(s, args);
        }
    }

}