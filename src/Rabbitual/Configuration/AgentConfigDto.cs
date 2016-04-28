namespace Rabbitual.Configuration
{
    public class AgentConfigDto
    {
        public AgentConfigDto()
        {
            SourceIds = new string[0];
        }
        public string Id { get; set; }
        public string Name { get; set; }
        public int? Schedule { get; set; }
        public string Type { get; set; }
        public string[] SourceIds { get; set; }
        public object Options { get; set; }
    }
}