namespace Rabbitual.Configuration
{
    public class AgentConfiguration : IAgentConfiguration
    {
        private readonly string _file;

        public AgentConfiguration(string file)
        {
            _file = file;
        }

        public AgentConfig[] GetConfiguration()
        {
            var config = new ConfigSerialization();
            return config.Get(_file);
        }
    }
}