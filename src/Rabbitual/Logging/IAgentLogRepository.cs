namespace Rabbitual.Logging
{
    public interface IAgentLogRepository
    {
        IAgentMessageLog GetLog(string agentId);
    }
}