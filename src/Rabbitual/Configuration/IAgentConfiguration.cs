namespace Rabbitual.Configuration
{
    public interface IAgentConfiguration
    {
        AgentConfig[] GetConfiguration();
        void PersistConfig(AgentConfigDto c);
    }
}