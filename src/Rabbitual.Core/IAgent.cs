namespace Rabbitual.Core
{
    /// <summary>
    /// An agent can perform tasks, recieve events or do something on a schedule.
    /// </summary>
    public interface IAgent
    {
        int Id { get; set; }

        void Start();

        /// <summary>
        /// Giving the agent a chance to persist its changes
        /// </summary>
        void Stop();
    }
    public interface IHaveOptions<TOption> where TOption:class{}


    /// <summary>
    /// An agent with with state that can seamlessly be fetched and persisted
    /// </summary>
    public interface IStatefulAgent<TState>: IAgent where TState : class    {  }

    /// <summary>
    ///     Start *something* on a schedule
    /// </summary>
    public interface IScheduledAgent : IAgent
    {
        int DefaultScheduleMs { get; }
        void Check();
    }


    /// <summary>
    ///     Consume event
    /// </summary>
    public interface IEventConsumerAgent : IAgent
    {
        void Consume(Message evt);
    }
}
