namespace Rabbitual.Configuration
{
    public interface IAgentConfiguration
    {
        AgentConfig[] GetConfiguration();
        void UpdateAgent(AgentConfigDto c);
        void InsertAgent(AgentConfigDto c);
    }
}