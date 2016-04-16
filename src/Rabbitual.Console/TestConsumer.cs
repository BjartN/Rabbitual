using System;

namespace Rabbitual.Console
{
    public class TestMessage:Message
    {
        public string Message
        {
            set { Data["Message"] = value; }
        }
    }

    public class TestConsumer : IConsumer
    {
        public bool CanConsume(object o)
        {
            return o is TestMessage;
        }

        public void Consume(object o)
        {
            System.Console.WriteLine("Consuming " + o.GetType());
        }
    }
}