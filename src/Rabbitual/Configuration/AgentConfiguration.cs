using System;
using System.Linq;
using Rabbitual.Infrastructure;
using Rabbitual.Logging;

namespace Rabbitual.Configuration
{
    public class AgentConfiguration : IAgentConfiguration
    {
        private readonly IAgentDb _agentDb;
        private readonly IJsonSerializer _s;
        private readonly ILogger _log;
        private readonly IConfigReflection _reflection;

        public AgentConfiguration(
            IAgentDb agentDb,
            IJsonSerializer s,
            ILogger log,
            IConfigReflection reflection)
        {
            _agentDb = agentDb;
            _s = s;
            _log = log;
            _reflection = reflection;
        }

        public AgentConfig[] GetConfiguration()
        {
            return _agentDb
                .GetAgents()
                .Select(toConfig)
                .ToArray();
        }

        public void UpdateAgent(AgentConfigDto c)
        {
            _log.Info($"Persisting agent {c.Id}");
            _agentDb.UpdateAgent(c);
        }

        public void InsertAgent(AgentConfigDto c)
        {
            _log.Info($"Persisting agent {c.Id}");
            _agentDb.InsertAgent(c);
        }

        private AgentConfig toConfig(AgentConfigDto c)
        {
            var map = _reflection.GetTypeMap();
            var d = new AgentConfig
            {
                Id = c.Id,
                ClrType = map[c.Type],
                Name = c.Name,
                Schedule = c.Schedule,
                SourceIds = c.SourceIds
            };

            var optionsType = OptionsHelper.GetOptionType(d.ClrType);
            if (optionsType != null)
            {
                var options = _agentDb.GetOptions(c.Id, optionsType);
                d.Options = options ?? Activator.CreateInstance(optionsType);
            }

            return d;
        }
    }
}