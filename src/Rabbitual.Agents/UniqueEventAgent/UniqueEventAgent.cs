using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbitual.Agents.UniqueEventAgent
{
    [Icon("hand-pointer-o")]
    public class UniqueEventAgent: ScheduledStatefulAgent<UniqueOptions,UniqueState>
        , IEventConsumerAgent
        , IEventPublisherAgent
    {
        private readonly IMessagePublisher _publisher;
        private readonly object _locker = new object();

        public UniqueEventAgent(
            UniqueOptions options, 
            IAgentStateRepository stateRepository, 
            IMessagePublisher publisher) : base(options, stateRepository)
        {
            _publisher = publisher;
            DefaultScheduleMs = 1000*60*60*1; //one hour
        }

        public void Consume(Message evt)
        {
            string uniqueIdentifier;

            if (!evt.Data.TryGetValue(Options.IdField, out uniqueIdentifier))
                return;

            if (State.Index.ContainsKey(uniqueIdentifier))
                return; //already seen this event

            lock(_locker)
                State.Index[uniqueIdentifier] = DateTime.UtcNow;

            _publisher.PublishEvent(new Message
            {
                Data = evt.Data
            });
        }

        public override void Check()
        {
            var old = DateTime.UtcNow.AddMinutes(-1*Options.KeepStateMinutes);

            lock (_locker)
            {
                //prune the list
                foreach (var item in State.Index.Where(p => p.Value < old).ToList())
                {
                    State.Index.Remove(item.Key);
                }
            }
        }
    }

    public class UniqueOptions
    {
        public string IdField { get; set; }
        public int KeepStateMinutes { get; set; }
    }

    public class UniqueState
    {
        public UniqueState()
        {
            Index= new Dictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string,DateTime> Index { get; set; }
    }
}