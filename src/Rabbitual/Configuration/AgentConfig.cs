using System;
using System.Linq;

namespace Rabbitual.Configuration
{
    public class AgentConfig
    {
        public AgentConfig()
        {
            SourceIds = new string[0];
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Schedule { get; set; }
        public Type ClrType { get; set; }
        public string[] SourceIds { get; set; }
        public object Options { get; set; }
        public AgentConfigDto ToDto()
        {
            return new AgentConfigDto
            {
                Id = Id,
                SourceIds = SourceIds,
                Schedule = Schedule,
                Options = Options,
                Name = Name,
                Type = ClrType.Name
            };
        }
    }
}