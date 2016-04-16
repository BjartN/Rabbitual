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

        public App(IMessageConsumer c, IAgent[] agents)
        {
            _c = c;
            _agents = agents;
        }

        public void Start()
        {
            //Note that an agent both on timer and waiting for events might get accessed concurrently

            //agents doing work on a timer
            foreach (var a in _agents.OfType<IScheduledAgent>())
            {
                new Timer().DoOnTimer(5000,() =>
                {
                    a.Check();
                });
            }

            //agents waiting for events
            _c.Start(_agents.OfType<IConsumerAgent>().ToArray());
        }

        public void Stop()
        {
            
        }
    }
}