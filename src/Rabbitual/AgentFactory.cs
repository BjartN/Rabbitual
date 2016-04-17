using System;
using System.Linq;
using Rabbitual.Configuration;

namespace Rabbitual
{
    public interface IAgentFactory
    {
        Tuple<IAgent,AgentConfig>[] GetAgents();
    }

    public class AgentFactory : IAgentFactory
    {
        private readonly Func<Type, IAgent> _agentFactory;
        private readonly IAgentConfiguration _cfg;

        public AgentFactory(Func<Type,IAgent> agentFactory, IAgentConfiguration cfg)
        {
            _agentFactory = agentFactory;
            _cfg = cfg;
        }

        public Tuple<IAgent,AgentConfig>[] GetAgents()
        {
            return _cfg.GetConfiguration()
                .Select(x => new Tuple<IAgent, AgentConfig>(_agentFactory(x.ClrType), x))
                .ToArray();
        } 
    }
}