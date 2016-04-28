using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Rabbitual.Configuration
{
    public class ConfigSerialization
    {
        public IDictionary<string, Type> GetTypeMap()
        {
            var type = typeof(IAgent);
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(p => type.IsAssignableFrom(p))
                .ToDictionary(x => x.Name, x => x);
        }

        public AgentConfig[] Get(string source)
        {
            var s = JsonConvert.DeserializeObject<AgentConfigDto[]>(File.ReadAllText(source));


            return ToConfig(s);
        }

        public AgentConfig[] ToConfig(AgentConfigDto[] dtos)
        {
            var map = GetTypeMap();
            var d = new Dictionary<string, AgentConfig>();

            foreach (var c in dtos)
            {
                d[c.Id] = new AgentConfig
                {
                    Id = c.Id,
                    ClrType = map[c.Type],
                    Name = c.Name,
                    Options = c.Options,
                    Schedule = c.Schedule
                };
            }

            foreach (var c in dtos)
            {
                var config = d[c.Id];
                config.Sources = c.SourceIds.Select(x => d[x]).ToArray();
            }

            foreach (var c in dtos)
            {
                var config = d[c.Id];
                var t = OptionsHelper.GetOptionType(config.ClrType);

                if (c.Options != null)
                {
                    var o = JsonConvert.DeserializeObject(((JObject)c.Options).ToString(), t);
                    config.Options = o;
                }

                config.Sources = c.SourceIds.Select(x => d[x]).ToArray();
            }

            return d.Select(x => x.Value).ToArray();
        }
    }
}