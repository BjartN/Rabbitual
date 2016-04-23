using System.ComponentModel;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents
{
    /// <summary>
    /// Publish an event on a schedule
    /// </summary>
    public class ScheduledPublisherAgent : Agent<ScheduledPublisherOptions>,
        IScheduledAgent, 
        IStatefulAgent<SweetState>,
        IPublishingAgent
    {
        private readonly ILogger _log;

        public ScheduledPublisherAgent(ILogger log)
        {
            _log = log;
        }

        public new void Stop()
        {
            StateService.PersistState(State);
        }

        public void Check()
        {
            State.Count++;
            Publisher.PublishEvent(new Message());
        }

        public int DefaultSchedule => 500;

        public IPublisher Publisher { get; set; }
        public SweetState State { get; set; }
        public IAgentState StateService { get; set; }
    }

    public class SweetState
    {
        public int Count { get; set; }
    }
    public class ScheduledPublisherOptions
    {
       
    }
}