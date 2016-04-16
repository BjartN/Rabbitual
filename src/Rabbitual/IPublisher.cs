namespace Rabbitual
{
    public interface IPublisher
    {
        void EnqueueTask(Message m, string queueName);
        void EnqueueTask(Message m);
        void PublishEvent(Message m, string exchangeName);
        void PublishEvent(Message m);
    }
}