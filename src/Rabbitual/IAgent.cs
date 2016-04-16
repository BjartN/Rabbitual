namespace Rabbitual
{
    /// <summary>
    /// An agent can perform tasks, recieve events or do something on a schedule.
    /// </summary>
    public interface IAgent
    {

    }

    /// <summary>
    ///     Start *something* on a schedule
    /// </summary>
    public interface IScheduledAgent : IAgent
    {
        void Check();
    }

    /// <summary>
    ///     Execute work 
    /// </summary>
    public interface ITaskConsumerAgent : IAgent
    {
        bool CanWorkOn(object task);
        void WorkOn(object task);
    }

    /// <summary>
    ///     Consume event
    /// </summary>
    public interface IConsumerAgent : IAgent
    {
        bool CanConsume(object evt);
        void Consume(object evt);
    }
}
