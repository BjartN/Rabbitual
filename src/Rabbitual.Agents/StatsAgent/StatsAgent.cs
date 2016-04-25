using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.StatsAgent
{
    public class StatsAgent : StatefulAgent<StatsOptions, StatsState>, 
        IEventConsumerAgent, 
        IEventPublisherAgent
    {
        private readonly ILogger _l;

        public StatsAgent( ILogger l,StatsOptions options,IAgentStateRepository repository) : base(options,repository)
        {
            _l = l;
        }

        public void Consume(Message evt)
        {
            State.Count = State.Count+1;
            _l.Debug($"Stats count is {State.Count}");

        }

    }

    public class StatsState 
    {
        public int Count { get; set; }
    }

    public class StatsOptions
    {
    }
}