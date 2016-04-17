using System;
using System.Collections.Generic;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents
{
    public class StatsAgent : IStatefulAgent, IEventConsumerAgent, IPublishingAgent
    {
        private readonly ILogger _l;
        private IAgentState _ctx;
        private StatsState _state;

        public StatsAgent( ILogger l)
        {
            _l = l;
        }

        public void Start(IAgentState ctx)
        {
            _ctx = ctx;
            _state = _ctx.GetState<StatsState>() ?? new StatsState();
        }

        public void Stop()
        {
            _ctx.PersistState(_state);
        }

        public void Consume(object evt)
        {
            var m = evt as Message;
            if (m == null)
                return;

            _state.Count = _state.Count+1;

            _l.Log($"All good. Count is {_state.Count}");

        }

        public IPublisher Publisher { get; set; }
        public string Id { get; set; }
    }

    public class StatsState
    {
        public int Count { get; set; }
    }
}