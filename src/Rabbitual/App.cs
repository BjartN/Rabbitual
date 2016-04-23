using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
   
    /// <summary>
    /// Starting point for everything. 
    /// </summary>
    public class App
    {
        private readonly IEventConsumer _eventConsumer;
        private readonly ITaskConsumer _taskConsumer;
        private readonly IAgentFactory _f;
        private readonly IObjectDb _db;
        private readonly ILogger _logger;
        private readonly IPublisher _p;
        private readonly List<Timer> _timers;
        private Ac[] _agents;

        public App(
            IEventConsumer eventConsumer,
            ITaskConsumer taskConsumer,
            IAgentFactory f,
            IObjectDb db,
            ILogger logger,
            IPublisher p)
        {
            _eventConsumer = eventConsumer;
            _taskConsumer = taskConsumer;
            _f = f;
            _db = db;
            _logger = logger;
            _p = p;
            _timers = new List<Timer>();
        }


        public void Start()
        {
            _agents = _f.GetAgents();

            //Note that an agent both on timer and waiting for events might get accessed concurrently
            foreach (var ac in _agents)
            {
                var agent = ac.Agent;
                var config = ac.Config;

                var agentWithOptions = agent as IHaveOptions;
                var statefulAgent = agent as IStatefulAgent;
                var scheduledAgent = agent as IScheduledAgent;
                var publishingAgent = agent as IPublishingAgent;

                //assign id to agent
                agent.Id = ac.Config.Id;

                if (agentWithOptions != null)
                {
                    //Set options
                    var options = config.Options ?? ReflectionHelper.CreateDefaultOptionsUsingMagic(agentWithOptions.GetType());
                    ReflectionHelper.SetOptionsUsingMagic(agentWithOptions, options);
                }

                if (publishingAgent != null)
                {
                    //Make sure that when things get published the publishing runs through a proxy so that we can enrich the message 
                    //with the id of the publisher.
                    publishingAgent.Publisher = new PublisherProxy(_p, publishingAgent.Id);
                }

                if (statefulAgent != null)
                {
                    //Inject state contex
                    var service = new AgentState(config.Id, _db, _logger);
                    statefulAgent.StateService = service;

                    //TODO: Set default state
                    var arg = ReflectionHelper.GetGenericArgument(statefulAgent.GetType(), typeof (IStatefulAgent<>));
                    var method = typeof(AgentState).GetMethod("GetState");
                    var genericMethod = method.MakeGenericMethod(arg);
                    var result = genericMethod.Invoke(service, null);
                    var pi = statefulAgent.GetType().GetProperty("State");
                    pi.SetValue(statefulAgent,result);

                }

                if (scheduledAgent != null)
                {
                    //Set agent on a timer, with some silly-checking.
                    var schedule = scheduledAgent.DefaultSchedule <= 0 ? 5000 : scheduledAgent.DefaultSchedule;
                    _timers.Push(new Timer(_logger)).Start(schedule, () => scheduledAgent.Check());
                }

                agent.Start();
            }

            //Agents waiting for events
            var eventConsumerAgents = _agents.Select(x => x.Agent).OfType<IEventConsumerAgent>().ToArray();
            var taskConsumerAgents = _agents.Select(x => x.Agent).OfType<ITaskConsumerAgent>().ToArray();

            _eventConsumer.Start(eventConsumerAgents);
            _taskConsumer.Start(taskConsumerAgents);
        }

        public void Stop()
        {
            _eventConsumer.Stop();
            _taskConsumer.Stop();

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