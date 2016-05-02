namespace Rabbitual
{
    public interface IAgentRepository
    {
        IAgentWrapper GetAgent(string agentId);
    }
}