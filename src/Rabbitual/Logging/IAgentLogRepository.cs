namespace Rabbitual.Logging
{
    public interface IAgentLogRepository
    {
        IAgentMessageLog GetLog(int agentId);
    }
}