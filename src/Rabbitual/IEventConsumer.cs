namespace Rabbitual
{
    public interface IEventConsumer
    {
        void Start(IEventConsumerAgent[] agents);
        void Stop();
    }

    public interface ITaskConsumer
    {
        void Start(ITaskConsumerAgent[] workers);
        void Stop();
    }

}