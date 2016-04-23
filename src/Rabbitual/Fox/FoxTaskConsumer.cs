using System.Collections.Generic;
using System.Linq;
using Rabbitual.Configuration;

namespace Rabbitual.Fox
{
    public class FoxTaskConsumer : ITaskConsumer
    {
        private readonly Hub _hub;
        private readonly IAgentConfiguration _cfg;
        private Dictionary<string, AgentConfig> _configs;

        public FoxTaskConsumer(Hub hub, IAgentConfiguration cfg)
        {
            _hub = hub;
            _cfg = cfg;
        }

        public void Start(ITaskConsumerAgent[] workers)
        {
            if (_configs == null)
                _configs = _cfg.GetConfiguration().ToDictionary(x => x.Id, x => x);

            foreach (var w in workers)
            {
                _hub.AddWorker(message =>
                {
                    if (!w.CanWorkOn(message))
                        //currently only way to signal that work is not suitable
                        throw new Fit();

                    w.DoWork(message);
                });
            }
        }

        public void Stop()
        {

        }
    }
}