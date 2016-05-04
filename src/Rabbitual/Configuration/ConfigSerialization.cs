using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using Rabbitual.Infrastructure;

namespace Rabbitual.Configuration
{
    public interface IConfigSerialization
    {
        AgentConfig[] Get(string source);
        AgentConfig[] Get(string[] source);
        AgentConfig[] ToConfig(AgentConfigDto[] dtos);
    }

    public class ConfigSerialization : IConfigSerialization
    {
        private readonly IJsonSerializer _s;
        private readonly IConfigReflection _reflection;

        public ConfigSerialization(IJsonSerializer s, IConfigReflection reflection)
        {
            _s = s;
            _reflection = reflection;
        }


        public AgentConfig[] Get(string source)
        {
            var s = _s.Deserialize<AgentConfigDto[]>(File.ReadAllText(source));

            return ToConfig(s);
        }


        public AgentConfig[] Get(string[] source)
        {
            var all =source.Select(s => _s.Deserialize<AgentConfigDto>(File.ReadAllText(s))).ToArray();

            return ToConfig(all);
        }

        public AgentConfig[] ToConfig(AgentConfigDto[] dtos)
        {
            var map = _reflection.GetTypeMap();
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
                config.Sources = c.SourceIds.Where(x=>d.ContainsKey(x)).Select(x => d[x]).ToArray();
            }

            foreach (var c in dtos)
            {
                var config = d[c.Id];
                var t = OptionsHelper.GetOptionType(config.ClrType);

                if (c.Options != null)
                {
                    //TODO: Making some internals assumptions here.
                    var o = _s.Deserialize(((JObject)c.Options).ToString(), t);
                    config.Options = o;
                }
            }

            return d.Select(x => x.Value).ToArray();
        }
    }
}