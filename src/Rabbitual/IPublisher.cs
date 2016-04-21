namespace Rabbitual
{
    /// <summary>
    ///     Delibarate separation between a task that requires ack before it can be removed from the task queue
    ///     vs and event that is fire an forget.
    /// 
    ///     TODO: Impement the task part of the equation
    /// </summary>
    public interface IPublisher
    {
        void EnqueueTask(Message task);
        void PublishEvent(Message e);
    }
}