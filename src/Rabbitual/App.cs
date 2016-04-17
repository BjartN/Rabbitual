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
        private readonly IOptionsRepository _options;
        private readonly List<Timer> _timers;
        private int _agentId;

        public App(IMessageConsumer c, IAgent[] agents, IObjectDb db, ILogger logger, IOptionsRepository options)
        {
            _c = c;
            _agents = agents;
            _db = db;
            _logger = logger;
            _options = options;
            _timers = new List<Timer>();
        }


        public void Start()
        {
            //Note that an agent both on timer and waiting for events might get accessed concurrently
            foreach (var a in _agents)
            {
                var oa = a as IOptionsAgent;
                var sa = a as IStatefulAgent;
                var sca = a as IScheduledAgent;

                if (oa != null)
                {
                    var options = _options.GetOptions(oa.GetType(), "0");

                    //set options using magic
                    ReflectionHelper.SetOptions(oa, options);
                }

                if (sa != null)
                {
                    sa.Start(new AgentState(_agentId.ToString(), _db, _logger));
                }

                if (sca != null)
                { 
                    _timers
                  .Push(new Timer())
                  .Start(5000, sca.Check);
                }

                _agentId++;
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