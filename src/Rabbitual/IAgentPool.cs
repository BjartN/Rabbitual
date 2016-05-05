namespace Rabbitual
{
    public interface IAgentPool
    {
        IAgentWrapper GetAgent(string agentId);
    }
}