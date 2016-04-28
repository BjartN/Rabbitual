using System;
using System.Linq;

namespace Rabbitual.Configuration
{
    public class AgentConfig
    {
        public AgentConfig()
        {
            Sources = new AgentConfig[0];
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Schedule { get; set; }
        public Type ClrType { get; set; }
        public AgentConfig[] Sources { get; set; }
        public object Options { get; set; }
        public AgentConfigDto ToDto()
        {
            return new AgentConfigDto
            {
                Id = Id,
                SourceIds = Sources.Select(x=>x.Id).ToArray(),
                Schedule = Schedule,
                Options = Options,
                Name = Name,
                Type = ClrType.Name
            };
        }
    }
}