using System.ComponentModel;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents
{
    /// <summary>
    /// Publish an event on a schedule
    /// </summary>
    public class ScheduledPublisherAgent : ScheduledStatefulAgent<ScheduledPublisherOptions,SweetState>,IPublishingAgent
    {
        private readonly ILogger _log;

        public ScheduledPublisherAgent(
            ILogger log, 
            ScheduledPublisherOptions options,
            IAgentStateRepository stateRepository,
            IPublisher publisher) : base(options,stateRepository)
        {
            Publisher = publisher;
            _log = log;
        }

        public override void Check()
        {
            State.Count++;
            Publisher.PublishEvent(new Message());
        }

        public new int DefaultSchedule => 500;

        public IPublisher Publisher { get; set; }
    }

    public class SweetState
    {
        public int Count { get; set; }
    }
    public class ScheduledPublisherOptions
    {
       
    }
}