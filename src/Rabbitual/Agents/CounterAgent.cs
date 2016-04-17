using System;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents
{
    public class CounterAgent: 
        IScheduledAgent,
        IEventConsumerAgent,
        IHaveOptions<CounterOptions>
    {
        private readonly ILogger _l;
        private int _count;

        public CounterAgent(ILogger l)
        {
            _l = l;
        }

        public int DefaultSchedule => 5000;

        public void Check()
        {
            _l.Log("I'm on a schedule. Have processed {0} events so far", _count);
        }

        public void Consume(object evt)
        {
            _count++;
        }

        public CounterOptions Options { get; set; }
        public string Id { get; set; }
    }

    public class CounterOptions
    {
    }
}