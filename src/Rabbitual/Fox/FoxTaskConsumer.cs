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

        public void Start(IAgentWrapper w)
        {
            if (_configs == null)
                _configs = _cfg.GetConfiguration().ToDictionary(x => x.Id, x => x);

            _hub.AddWorker(w);
        }

        public void Stop()
        {

        }
    }
}