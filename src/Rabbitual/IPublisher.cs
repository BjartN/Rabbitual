namespace Rabbitual
{
    /// <summary>
    ///     Delibarate separation between a task that requires ack before it can be removed from the task queue
    ///     vs and event that is fire an forget
    /// </summary>
    public interface IPublisher
    {
        void SubmitTask(Message m, string queueName);
        void SubmitTask(Message m);

        void PublishEvent(Message m, string exchangeName);
        void PublishEvent(Message m);
    }
}