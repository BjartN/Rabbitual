namespace Rabbitual.Core
{
    public interface IMessagePublisher
    {
        void EnqueueTask(Message task);
        void PublishEvent(Message e);
    }
}