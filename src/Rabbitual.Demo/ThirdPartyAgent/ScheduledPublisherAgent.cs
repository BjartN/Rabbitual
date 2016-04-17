using System.ComponentModel;
using Rabbitual.Infrastructure;

namespace Rabbitual.Demo.ThirdPartyAgent
{
    /// <summary>
    /// Publish an event on a schedule
    /// </summary>
    public class ScheduledPublisherAgent :
        IScheduledAgent, 
        IOptionsAgent<ScheduledPublisherAgentOptions>,
        IStatefulAgent
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
            _log.Log("Publishing an event for the {0} time", _state.Count);
            _p.PublishEvent(new Message());
        }


        public class SweetState
        {
            public int Count { get; set; }
        }

        public ScheduledPublisherAgentOptions Options { set; get; }
    }

    public class ScheduledPublisherAgentOptions
    {
        public ScheduledPublisherAgentOptions()
        {
            Schedule = 100; //default value
        }

        [Description(@"
                This exciting agent count all events in 
                the system and logs the count on a schedule.")]
        public int Schedule { get; set; }
    }
}