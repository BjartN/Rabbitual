using System;
using System.Linq;
using System.Security.AccessControl;
using Rabbitual.Configuration;

namespace Rabbitual
{
    public class Ac //in lack of a better name
    {
        public Ac(IAgent agent, AgentConfig config)
        {
            Agent = agent;
            Config = config;
        }

        public AgentConfig Config{ get;private set; }
        public IAgent Agent{ get; private set; }

    }

    public interface IAgentFactory
    {
        Ac[] GetAgents();
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

        public Ac[] GetAgents()
        {
            return _cfg.GetConfiguration()
                .Select(x => new Ac((IAgent)_agentFactory.GetInstance(x.ClrType), x))
                .ToArray();
        } 
    }
}