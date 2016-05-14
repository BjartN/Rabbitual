using System;
using System.Linq;
using Rabbitual.Core.Logging;

namespace Rabbitual.Configuration
{
    public class AgentConfiguration : IAgentConfiguration
    {
        private readonly IAgentDb _agentDb;
        private readonly ILogger _log;
        private readonly IConfigReflection _reflection;

        public AgentConfiguration(
            IAgentDb agentDb,
            ILogger log,
            IConfigReflection reflection)
        {
            _agentDb = agentDb;
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
            _log.Info($"Updating agent {c.Id}");
            _agentDb.UpdateAgent(c);
        }

        public void InsertAgent(AgentConfigDto c)
        {
            _log.Info($"Inserting agent {c.Id}");
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