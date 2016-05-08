namespace Rabbitual
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
    ///    Publishig agent
    /// </summary>
    public interface IEventPublisherAgent : IAgent { }


    /// <summary>
    ///     Execute work 
    /// </summary>
    public interface ITaskConsumerAgent : IAgent
    {
        bool CanDoWork(Message task);
        void DoWork(Message task);
    }

    /// <summary>
    ///     Consume event
    /// </summary>
    public interface IEventConsumerAgent : IAgent
    {
        void Consume(Message evt);
    }


    public interface IAgentStateRepository
    {
        T GetState<T>();
        void PersistState(object state);
    }
}
