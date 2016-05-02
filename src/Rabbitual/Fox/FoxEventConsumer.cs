using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Configuration;

namespace Rabbitual.Fox
{
    public class FoxEventConsumer : IEventConsumer
    {
        private readonly Hub _hub;

        public FoxEventConsumer(Hub hub)
        {
            _hub = hub;
        }

        public void Start(IAgentWrapper a)
        {
            _hub.Subscribe(a);
        }
    }
}