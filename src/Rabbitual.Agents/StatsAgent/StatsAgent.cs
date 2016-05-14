using Rabbitual.Core;
using Rabbitual.Core.Logging;

namespace Rabbitual.Agents.StatsAgent
{
    /// <summary>
    /// Count number of events
    /// </summary>
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
            _l.Debug($"Event count count is {State.Count}");
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