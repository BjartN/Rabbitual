namespace Rabbitual
{
    public interface IMessageConsumer
    {
        void Start(IEventConsumerAgent[] agents);
        void Stop();
    }
}