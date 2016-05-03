using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Infrastructure;
using Rabbitual.Logging;

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
        private IAgentWrapper[] _agents = new IAgentWrapper[0];

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

        public IAgentWrapper GetAgent(string id)
        {
            return _agents.FirstOrDefault(x => x.Id == id);
        }

        public void Start()
        {
            _agents = _f.GetAgents();

            foreach (var ac in _agents)
            {
                RunAgent(ac);
            }
        }

        public void RunAgent(IAgentWrapper agent)
        {
            if (agent.IsScheduled())
            {
                var schedule = agent.GetSchedule();
                schedule = schedule <= 0 ? 5000 : schedule;
                _timers.Push(new Timer(_logger)).Start(schedule, agent.Check);
            }
            if (agent.IsConsumer())
            {
                _eventConsumer.Start(agent);
            }
            if (agent.IsWorker())
            {
                _taskConsumer.Start(agent);
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

            foreach (var a in _agents.Select(x => x))
            {
                a.Stop();
            }
        }
    }
}