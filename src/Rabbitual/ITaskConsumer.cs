namespace Rabbitual
{
    public interface ITaskConsumer
    {
        void Start(IAgentWrapper worker);
    }
}