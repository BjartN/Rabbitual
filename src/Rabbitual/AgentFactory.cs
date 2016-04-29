﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    public class Ac //in lack of a better name
    {
        public Ac(IAgentWrapper agent, AgentConfig config)
        {
            Agent = agent;
            Config = config;
        }

        public AgentConfig Config { get; private set; }
        public IAgentWrapper Agent { get; private set; }

    }

    public interface IAgentFactory
    {
        Ac[] GetAgents();
    }

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
        private readonly IPublisher _p;
        private readonly IAgentLogRepository _agentLog;

        public AgentFactory(
            ILogger logger,
            IObjectDb db,
            IFactory factory,
            IAgentConfiguration cfg,
            IPublisher p,
            IAgentLogRepository agentLog)
        {
            _logger = logger;
            _db = db;
            _factory = factory;
            _cfg = cfg;
            _p = p;
            _agentLog = agentLog;
        }

        public Ac[] GetAgents()
        {
            var cfg = _cfg.GetConfiguration();

            return cfg.Select(x => new Ac(createAgent(x), x))
                .ToArray();
        }

        private IAgentWrapper createAgent(AgentConfig config)
        {
            //TODO: Inject agent specific logger

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
                deps.Add(typeof(IPublisher), new PublisherProxy(_p, config.Id, _agentLog));
            }

            if (includeState)
            {
                var agentState = new AgentStateRepository(config.Id, _db, _logger);
                deps.Add(typeof(IAgentStateRepository), agentState);
            }

            var al = _agentLog.GetLog(config.Id);

            var agent = (IAgent)_factory.GetInstance(agentType, deps);
            agent.Id = config.Id;

            return new AgentWrapper(agent,al,_logger);
        }
    }
}