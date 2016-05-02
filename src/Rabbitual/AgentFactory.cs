using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
   
    public interface IAgentFactory
    {
        IAgentWrapper[] GetAgents();
    }

    /// <summary>
    /// Used for creating objects dynamically after the application has started.
    /// </summary>
    public interface IFactory
    {
        object GetInstance(Type t, IDictionary<Type, object> deps = null);
    }

    public class AgentFactory : IAgentFactory
    {
        private readonly ILogger _logger;
        private readonly IObjectDb _db;
        private readonly IFactory _factory;
        private readonly IAgentConfiguration _cfg;
        private readonly IMessagePublisher _p;
        private readonly IAgentLogRepository _agentLog;

        public AgentFactory(
            ILogger logger,
            IObjectDb db,
            IFactory factory,
            IAgentConfiguration cfg,
            IMessagePublisher p,
            IAgentLogRepository agentLog)
        {
            _logger = logger;
            _db = db;
            _factory = factory;
            _cfg = cfg;
            _p = p;
            _agentLog = agentLog;
        }

        public IAgentWrapper[] GetAgents()
        {
            var cfg = _cfg.GetConfiguration();

            return cfg.Select(createAgent).ToArray();
        }

        /// <summary>
        /// Determine agent dependencies, create then and then create agent.
        /// </summary>
        private IAgentWrapper createAgent(AgentConfig config)
        {
            var agentType = config.ClrType;
            var includeOptions = agentType.IsOfType(typeof(IHaveOptions<>));
            var includeState = agentType.IsOfType(typeof(IStatefulAgent<>));
            var inculdePublisher = agentType.IsOfType(typeof(IEventPublisherAgent));
            var deps = new Dictionary<Type, object>();

            if (includeOptions)
            {
                var optionType = OptionsHelper.GetOptionType(agentType);
                deps.Add(optionType, config.Options ?? Activator.CreateInstance(optionType));
            }

            if (inculdePublisher)
            {
                deps.Add(typeof(IMessagePublisher), new PublisherProxy(_p, config.Id, _agentLog));
            }

            if (includeState)
            {
                var agentState = new AgentStateRepository(config.Id, _db, _logger);
                deps.Add(typeof(IAgentStateRepository), agentState);
            }

            var agentLog = _agentLog.GetLog(config.Id);
            var agent = (IAgent)_factory.GetInstance(agentType, deps);
            agent.Id = config.Id;

            return new AgentWrapper(agent, config,agentLog,_logger);
        }
    }
}