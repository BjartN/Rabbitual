using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;
using Rabbitual.Logging;

namespace Rabbitual
{
   
    public interface IAgentFactory
    {
        IAgentWrapper[] GetAgents();
    }

    public class AgentFactory : IAgentFactory
    {
        private readonly IAgentDb _db;
        private readonly ILogger _logger;
        private readonly IFactory _factory;
        private readonly IAgentConfiguration _cfg;
        private readonly IMessagePublisher _p;
        private readonly IAgentLogRepository _agentLog;

        public AgentFactory(
            IAgentDb db,
            ILogger logger,
            IFactory factory,
            IAgentConfiguration cfg,
            IMessagePublisher p,
            IAgentLogRepository agentLog)
        {
            _db = db;
            _logger = logger;
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
                var options = _db.GetOptions(config.Id, optionType);
                deps.Add(optionType, options ?? Activator.CreateInstance(optionType));
            }

            if (inculdePublisher)
            {
                deps.Add(typeof(IMessagePublisher), new AgentPublisher(_p, config.Id, _agentLog));
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