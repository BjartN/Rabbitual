namespace Rabbitual.Rabbit
{
    public interface IMessageConsumer
    {
        void Start(IConsumerAgent[] agents);
    }
}