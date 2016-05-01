using System.IO;
using System.Linq;
using Rabbitual.Infrastructure;

namespace Rabbitual.Configuration
{
    public class AgentConfiguration : IAgentConfiguration
    {
        private readonly ConfigSerialization _configSerialization;
        private readonly IObjectDb _db;
        private readonly ILogger _log;
        private readonly string _folder;

        public AgentConfiguration(IAppConfiguration cfg, ConfigSerialization configSerialization,IObjectDb db, ILogger log)
        {
            _configSerialization = configSerialization;
            _db = db;
            _log = log;
            _folder = cfg.Get("rabbitual.filedb.folder");
        }

        public AgentConfig[] GetConfiguration()
        {
            var result = Directory
                .GetFiles(_folder, "*.json")
                .Where(x => new FileInfo(x).Name.StartsWith("agent."))
                .ToArray();

            return _configSerialization.Get(result);
        }

        public void PersistConfig(AgentConfigDto c)
        {
            _log.Info($"Persisting agent {c.Id}");
            _db.Save(c, "agent." + c.Id);
        }
    }
}