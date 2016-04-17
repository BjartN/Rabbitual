using System.Collections.Generic;
using System.Linq;
using Rabbitual.Configuration;

namespace Rabbitual.Fox
{
    public class FoxMessageConsumer : IMessageConsumer
    {
        private readonly MessageHub _m;
        private readonly IAgentConfiguration _cfg;
        private Dictionary<string, AgentConfig> _configs;

        public FoxMessageConsumer(MessageHub m, IAgentConfiguration cfg)
        {
            _m = m;
            _cfg = cfg;
        }

        public void Start(IEventConsumerAgent[] agents)
        {
            if (_configs == null)
                _configs = _cfg.GetConfiguration().ToDictionary(x => x.Id, x => x);

            foreach (var a in agents)
            {
                var cfg = _configs[a.Id];

                _m.Subscribe(message =>
                {
                    //TODO: This is slow. Fix at a suitable time.
                    var noSourceMatch = cfg.Sources.Any() && cfg.Sources.All(s => s.Id != message.SourceAgentId);
                    if (noSourceMatch)
                        return;

                    a.Consume(message);
                });
            }
        }

        public void Stop()
        {

        }
    }
}