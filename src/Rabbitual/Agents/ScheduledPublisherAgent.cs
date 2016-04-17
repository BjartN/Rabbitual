using System.ComponentModel;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents
{
    /// <summary>
    /// Publish an event on a schedule
    /// </summary>
    public class ScheduledPublisherAgent :
        IScheduledAgent, 
        IHaveOptions<ScheduledPublisherOptions>,
        IStatefulAgent,
        IPublishingAgent
    {
        private readonly ILogger _log;
        private SweetState _state;
        private IAgentState _cfx;

        public ScheduledPublisherAgent(ILogger log)
        {
            _log = log;
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
            Publisher.PublishEvent(new Message());
        }

        public int DefaultSchedule => 500;


        public class SweetState
        {
            public int Count { get; set; }
        }

        public ScheduledPublisherOptions Options { set; get; }
        public IPublisher Publisher { get; set; }
        public string Id { get; set; }
    }

    public class ScheduledPublisherOptions
    {
       
    }
}