﻿using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Configuration;

namespace Rabbitual.Fox
{
    public class FoxEventConsumer : IEventConsumer
    {
        private readonly Hub _m;
        private readonly IAgentConfiguration _cfg;
        private Dictionary<string, AgentConfig> _configs;

        public FoxEventConsumer(Hub m, IAgentConfiguration cfg)
        {
            _m = m;
            _cfg = cfg;
        }

        public void Start(IAgentWrapper a)
        {
            if (_configs == null)
                _configs = _cfg.GetConfiguration().ToDictionary(x => x.Id, x => x);

            var cfg = _configs[a.Id];

            _m.Subscribe(message =>
            {
                //TODO: This is slow. 
                var noSourceMatch = cfg.Sources.Any() && cfg.Sources.All(s => s.Id != message.SourceAgentId);
                if (noSourceMatch)
                    return;
                a.Consume(message);
            }, a.Id);
        }

    }
}