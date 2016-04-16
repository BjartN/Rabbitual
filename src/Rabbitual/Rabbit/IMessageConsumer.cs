namespace Rabbitual.Rabbit
{
    public interface IMessageConsumer
    {
        void Start(IEventConsumerAgent[] agents);
        void Stop();
    }
}