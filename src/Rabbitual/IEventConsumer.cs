namespace Rabbitual
{
    public interface IEventConsumer
    {
        void Start(IEventConsumerAgent agent);
    }

    public interface ITaskConsumer
    {
        void Start(ITaskConsumerAgent worker);
    }

}