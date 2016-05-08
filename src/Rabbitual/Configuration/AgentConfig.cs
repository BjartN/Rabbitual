using System;
using System.Linq;

namespace Rabbitual.Configuration
{
    public class AgentConfig
    {
        public AgentConfig()
        {
            SourceIds = new int[0];
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Schedule { get; set; }
        public Type ClrType { get; set; }
        public int[] SourceIds { get; set; }
        public object Options { get; set; }
        public AgentConfigDto ToDto()
        {
            return new AgentConfigDto
            {
                Id = Id,
                SourceIds = SourceIds,
                Schedule = Schedule,
                Name = Name,
                Type = ClrType.Name
            };
        }
    }
}