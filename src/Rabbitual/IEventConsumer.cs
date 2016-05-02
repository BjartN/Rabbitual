namespace Rabbitual
{
    public interface IEventConsumer
    {
        void Start(IAgentWrapper agent);
    }
}