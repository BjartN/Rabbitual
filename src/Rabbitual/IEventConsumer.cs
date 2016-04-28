namespace Rabbitual
{
    public interface IEventConsumer
    {
        void Start(IAgentWrapper agent);
    }

    public interface ITaskConsumer
    {
        void Start(IAgentWrapper worker);
    }

}