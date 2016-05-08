namespace Rabbitual
{
    public interface IAgentPool
    {
        IAgentWrapper GetAgent(int agentId);
    }
}