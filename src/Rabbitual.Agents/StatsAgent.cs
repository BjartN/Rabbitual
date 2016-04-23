using Rabbitual.Infrastructure;

namespace Rabbitual.Agents
{
    public class StatsAgent : StatefulAgent<StatsOptions, StatsState>, 
        IEventConsumerAgent, 
        IPublishingAgent
    {
        private readonly ILogger _l;

        public StatsAgent( ILogger l)
        {
            _l = l;
        }

        public void Consume(object evt)
        {
            var m = evt as Message;
            if (m == null)
                return;

            State.Count = State.Count+1;
            _l.Debug($"Stats count is {State.Count}");

        }

        public IPublisher Publisher { get; set; }
    }

    public class StatsState 
    {
        public int Count { get; set; }
    }

    public class StatsOptions
    {
    }
}