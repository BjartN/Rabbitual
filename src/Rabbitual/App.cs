using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    /// <summary>
    /// Starting point for everything. 
    /// </summary>
    public class App : IAgentRepository
    {
        private readonly IEventConsumer _eventConsumer;
        private readonly ITaskConsumer _taskConsumer;
        private readonly IAgentFactory _f;
        private readonly ILogger _logger;
        private readonly List<Timer> _timers;
        private Ac[] _agents = new Ac[0];

        public App(
            IEventConsumer eventConsumer,
            ITaskConsumer taskConsumer,
            IAgentFactory f,
            ILogger logger)
        {
            _eventConsumer = eventConsumer;
            _taskConsumer = taskConsumer;
            _f = f;
            _logger = logger;
            _timers = new List<Timer>();
        }

        public Ac GetAgent(string id)
        {
            return _agents.FirstOrDefault(x => x.Agent.Id == id);
        }

        public void Start()
        {
            _agents = _f.GetAgents();

            //Note that an agent both on timer and waiting for events might get accessed concurrently
            foreach (var ac in _agents)
            {
                RunAgent(ac);
            }
        }

        public void RunAgent(Ac ac)
        {
            var agent = ac.Agent;
            var scheduledAgent = agent as IScheduledAgent;
            var eventConsumerAgent = agent as IEventConsumerAgent;
            var taskConsumerAgent = agent as ITaskConsumerAgent;

            agent.Id = ac.Config.Id;

            if (scheduledAgent != null)
            {
                //Set agent on a timer, with some silly-checking.
                var schedule = scheduledAgent.DefaultScheduleMs <= 0 ? 5000 : scheduledAgent.DefaultScheduleMs;
                _timers.Push(new Timer(_logger)).Start(schedule, () => scheduledAgent.Check());
            }
            if (eventConsumerAgent != null)
            {
                _eventConsumer.Start(eventConsumerAgent);
            }
            if (taskConsumerAgent != null)
            {
                _taskConsumer.Start(taskConsumerAgent);
            }

            agent.Start();
        }

        public void Stop()
        {
            //TODO: Should shut down message flow through hub

            foreach (var t in _timers)
            {
                t.Stop();
            }

            foreach (var a in _agents.Select(x => x.Agent))
            {
                a.Stop();
            }
        }
    }
}