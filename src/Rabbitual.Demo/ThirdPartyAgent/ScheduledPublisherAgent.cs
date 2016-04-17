using System.Collections.Generic;
using Rabbitual.Infrastructure;

namespace Rabbitual.Demo.ThirdPartyAgent
{
    /// <summary>
    /// Publish an event on a schedule
    /// </summary>
    public class ScheduledPublisherAgent: IScheduledAgent, IOptionsAgent, IStatefulAgent
    {
        private readonly ILogger _log;
        private readonly IPublisher _p;
        private SweetState _state;
        private IAgentState _cfx;

        public ScheduledPublisherAgent(ILogger log, IPublisher p)
        {
            _log = log;
            _p = p;
        }

        public void Start(IAgentState ctx)
        {
            _cfx = ctx;
            _state = ctx.GetState<SweetState>() ?? new SweetState();
        }

        public void Stop()
        {
            _cfx.PersistState(_state);
        }

        public void Check()
        {
            _state.Count++;
            _log.Log("Pubslihing an event for the {0} time", _state.Count);
            _p.PublishEvent(new Message());
        }

        public IDictionary<string, string> Options
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {"options.schedule", "100"}
                };
            }
        }

    }

    public class SweetState
    {
        public int Count { get; set; }
    }
}