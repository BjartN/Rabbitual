using System;

namespace Rabbitual.Agents
{
    public class CounterAgent: IScheduledAgent, IConsumerAgent
    {
        private readonly ILogger _l;
        private int _count;

        public CounterAgent(ILogger l)
        {
            _l = l;
        }

        public void Check()
        {
            _l.Log("I'm on a schedule. Have processed {0} events so far", _count);
        }

        public bool CanConsume(object evt)
        {
            return true;
        }

        public void Consume(object evt)
        {
            _count++;
        }

    }
}