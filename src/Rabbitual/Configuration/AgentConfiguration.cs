using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Rabbitual.Infrastructure;
using Rabbitual.Logging;

namespace Rabbitual.Configuration
{
    public class AgentConfiguration : IAgentConfiguration
    {
        private readonly IObjectDb _db;
        private readonly IJsonSerializer _s;
        private readonly ILogger _log;
        private readonly IConfigReflection _reflection;
        private readonly string _folder;

        public AgentConfiguration(
            IAppConfiguration cfg, 
            IObjectDb db, 
            IJsonSerializer s,
            ILogger log,
            IConfigReflection reflection)
        {
            _db = db;
            _s = s;
            _log = log;
            _reflection = reflection;
            _folder = cfg.Get("rabbitual.filedb.folder");
        }

        public AgentConfig[] GetConfiguration()
        {
            var result = Directory
                .GetFiles(_folder, "*.json")
                .Where(x => new FileInfo(x).Name.StartsWith("agent."))
                .ToArray();

            return result.Select(toConfig).ToArray();
        }

        public void PersistConfig(AgentConfigDto c)
        {
            _log.Info($"Persisting agent {c.Id}");
            _db.Save(c, "agent." + c.Id);
        }

        private AgentConfig toConfig(string path)
        {
            var c = _s.Deserialize<AgentConfigDto>(File.ReadAllText(path));

            var map = _reflection.GetTypeMap();
            var d = new AgentConfig
            {
                Id = c.Id,
                ClrType = map[c.Type],
                Name = c.Name,
                Options = c.Options,
                Schedule = c.Schedule,
                SourceIds = c.SourceIds
            };

            var t = OptionsHelper.GetOptionType(d.ClrType);
            if (c.Options != null)
            {
                //TODO: Making some internals assumptions here.
                var o = _s.Deserialize(((JObject)c.Options).ToString(), t);
                d.Options = o;
            }

            return d;
        }
    }
}