using System;
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

        public AgentConfig Config{ get;private set; }
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
        private readonly IAgentService _s;
        private readonly IPublisher _p;
        private readonly IAgentLogRepository _agentLog;

        public AgentFactory(
            ILogger logger,
            IObjectDb db,
            IFactory factory, 
            IAgentConfiguration cfg,
            IAgentService s,
            IPublisher p, IAgentLogRepository agentLog)
        {
            _logger = logger;
            _db = db;
            _factory = factory;
            _cfg = cfg;
            _s = s;
            _p = p;
            _agentLog = agentLog;
        }

        public Ac[] GetAgents()
        {
            var cfg =_cfg.GetConfiguration();

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
            var deps = new Dictionary<Type,object>();

            if (includeOptions)
            {
                var optionType = OptionsHelper.GetOptionType(agentType);
                deps.Add(optionType,config.Options ?? Activator.CreateInstance(optionType));
            }

            if (inculdePublisher)
            {
                deps.Add(typeof(IPublisher),new PublisherProxy(_p, config.Id, _agentLog));
            }

            if (includeState)
            {
                var agentState = new AgentStateRepository(config.Id, _db, _logger);
                deps.Add(typeof(IAgentStateRepository),agentState);
            }

            var agent = _factory.GetInstance(agentType,deps);

            return new AgentWrapper((IAgent)agent);
        }
    }

    public interface IAgentWrapper
    {
        string Id { get; set; }
        
        //Statefullness
        bool HasState();
        object GetState();

        void Start();
        void Stop();
        void Check();

        //Scheduling
        bool IsScheduled();
        int GetSchedule();

        //Pub Sub
        bool IsPublisher();
        bool IsConsumer();
        void Consume(Message message);


        //Producer consumer
        bool IsWorker();
        bool CanDoWork(Message message);
        void DoWork(Message message);
    }

    public class AgentWrapper : IAgentWrapper
    {
        private readonly IAgent _agent;

        public AgentWrapper(IAgent agent)
        {
            _agent = agent;
            Id = _agent.Id;
        }

        public string Id { get; set; }

        public bool IsWorker()
        {
            return _agent is ITaskConsumerAgent;
        }

        public bool CanDoWork(Message message)
        {
            return ((ITaskConsumerAgent)_agent).CanDoWork(message);
        }

        public void DoWork(Message message)
        {
            ((ITaskConsumerAgent)_agent).DoWork(message);
        }

        public void Consume(Message message)
        {
            ((IEventConsumerAgent)_agent).Consume(message);
        }

        public bool IsScheduled()
        {
            return _agent is IScheduledAgent;
        }

        public int GetSchedule()
        {
           return ((IScheduledAgent) _agent).DefaultScheduleMs;
        }

        public bool IsPublisher()
        {
            return _agent is IEventPublisherAgent;
        }

        public object GetState()
        {
            return StateHelper.GetStateUsingMagic(_agent);
        }

        public void Start()
        {
            _agent.Start();
        }

        public void Stop()
        {
            _agent.Stop();
        }

        public void Check()
        {
           ((IScheduledAgent)_agent).Check();
        }

        public bool IsConsumer()
        {
            return _agent is IEventConsumerAgent;
        }

        public bool HasState()
        {
            return _agent.GetType().IsOfType(typeof(IStatefulAgent<>));
        }
    }
}