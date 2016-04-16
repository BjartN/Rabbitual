using System.Collections.Generic;
using System.Linq;
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
        private readonly List<Timer> _timers;

        public App(IMessageConsumer c, IAgent[] agents)
        {
            _c = c;
            _agents = agents;
            _timers = new List<Timer>();
        }

        public void Start()
        {
            //Note that an agent both on timer and waiting for events might get accessed concurrently

            //agents doing work on a timer
            foreach (var a in _agents.OfType<IScheduledAgent>())
            {
                var timer = new Timer();
                timer.Start(5000,a.Check);
                _timers.Add(timer);
            }

            //agents waiting for events
            _c.Start(_agents.OfType<IConsumerAgent>().ToArray());
        }

        public void Stop()
        {
            _c.Stop();
            foreach(var t in _timers) 
                t.Stop();
        }
    }
}