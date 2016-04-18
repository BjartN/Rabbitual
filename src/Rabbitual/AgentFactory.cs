using System;
using System.Linq;
using Rabbitual.Configuration;

namespace Rabbitual
{
    public interface IAgentFactory
    {
        Tuple<IAgent,AgentConfig>[] GetAgents();
    }

    public interface IFactory
    {
        object GetInstance(Type t);
    }

    public class AgentFactory : IAgentFactory
    {
        private readonly IFactory _agentFactory;
        private readonly IAgentConfiguration _cfg;

        public AgentFactory(IFactory agentFactory, IAgentConfiguration cfg)
        {
            _agentFactory = agentFactory;
            _cfg = cfg;
        }

        public Tuple<IAgent,AgentConfig>[] GetAgents()
        {
            return _cfg.GetConfiguration()
                .Select(x => new Tuple<IAgent, AgentConfig>((IAgent)_agentFactory.GetInstance(x.ClrType), x))
                .ToArray();
        } 
    }
}