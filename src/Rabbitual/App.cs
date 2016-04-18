using System;
using System.Collections.Generic;
using System.Linq;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual
{
    /*
     *  RabbitMQ terminology:
     *      A producer is a user application that sends messages.
     *      A consumer is a user application that receives messages.
     *  
     *  Note:
     *  An agent can be both a consmer and producer at the same time. 
     */

    /// <summary>
    /// Starting point for everything. 
    /// </summary>
    public class App
    {
        private readonly IMessageConsumer _c;
        private readonly IAgentFactory _f;
        private readonly IObjectDb _db;
        private readonly ILogger _logger;
        private readonly IPublisher _p;
        private readonly List<Timer> _timers;
        private Tuple<IAgent, AgentConfig>[] _agents;

        public App(
            IMessageConsumer c,
            IAgentFactory f,
            IObjectDb db,
            ILogger logger,
            IPublisher p)
        {
            _c = c;
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
            foreach (var tuple in _agents)
            {
                var agent = tuple.Item1;
                var config = tuple.Item2;

                var agentWithOptions = agent as IHaveOptions;
                var statefulAgent = agent as IStatefulAgent;
                var scheduledAgent = agent as IScheduledAgent;
                var publishingAgent = agent as IPublishingAgent;

                //assign id to agent
                agent.Id = tuple.Item2.Id;

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
                    //Inject state context
                    statefulAgent.Start(new AgentState(config.Id, _db, _logger));
                }

                if (scheduledAgent != null)
                {
                    //Set agent on a timer, with some silly-checking.
                    var schedule = scheduledAgent.DefaultSchedule <= 0 ? 5000 : scheduledAgent.DefaultSchedule;
                    _timers.Push(new Timer()).Start(schedule, () => scheduledAgent.Check());
                }
            }

            //Agents waiting for events
            _c.Start(_agents.Select(x => x.Item1).OfType<IEventConsumerAgent>().ToArray());
        }

        public void Stop()
        {
            _c.Stop();
            foreach (var t in _timers)
            {
                t.Stop();
            }

            foreach (var a in _agents.Select(x => x.Item1).OfType<IStatefulAgent>())
            {
                a.Stop();
            }
        }
    }
}