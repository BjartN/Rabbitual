using System.Collections.Generic;
using System.Linq;
using Rabbitual.Infrastructure;
using Rabbitual.Rabbit;

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
        private readonly IAgent[] _agents;
        private readonly IObjectDb _db;
        private readonly ILogger _logger;
        private readonly List<Timer> _timers;
        private int _uniqueId;

        public App(IMessageConsumer c, IAgent[] agents, IObjectDb db, ILogger logger)
        {
            _c = c;
            _agents = agents;
            _db = db;
            _logger = logger;
            _timers = new List<Timer>();
        }


        public void Start()
        {
            //Note that an agent both on timer and waiting for events might get accessed concurrently

            //start all statefull agents
            foreach (var a in _agents.OfType<IStatefulAgent>())
            {
                a.Start(new AgentState((_uniqueId++).ToString(), _db, _logger));
            }

            //agents doing work on a timer
            foreach (var a in _agents.OfType<IScheduledAgent>())
            {
                var interval = a.TryGetIntOption("options.schedule") ?? 5000;

                var timer = new Timer();
                timer.Start(interval, a.Check);
                _timers.Add(timer);
            }

            //agents waiting for events
            _c.Start(_agents.OfType<IEventConsumerAgent>().ToArray());
        }

        public void Stop()
        {
            _c.Stop();
            foreach (var t in _timers)
            {
                t.Stop();
            }

            foreach (var a in _agents.OfType<IStatefulAgent>())
            {
                a.Stop();
            }
        }
    }
}