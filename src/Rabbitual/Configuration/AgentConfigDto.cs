using Newtonsoft.Json;

namespace Rabbitual.Configuration
{
    public class AgentConfigBaseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? Schedule { get; set; }
        public string Type { get; set; }

        public AgentConfigDto ToConfig(int[] sources)
        {
            return new AgentConfigDto
            {
                Id = Id,
                Name = Name,
                Schedule = Schedule,
                Type = Type,
                SourceIds = sources
            };
        }
    }

    public class AgentConfigDto:AgentConfigBaseDto
    {
        public AgentConfigDto()
        {
            SourceIds = new int[0];
        }
        public int[] SourceIds { get; set; }
    }
}