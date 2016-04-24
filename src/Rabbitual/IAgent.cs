using System.Collections.Generic;
using System.Security.AccessControl;
using RabbitMQ.Client.Framing.Impl;
using Rabbitual.Fox;

namespace Rabbitual
{
    /// <summary>
    /// An agent can perform tasks, recieve events or do something on a schedule.
    /// </summary>
    public interface IAgent
    {
        string Id { get; set; }

        void Start();

        /// <summary>
        /// Giving the agent a chance to persist its changes
        /// </summary>
        void Stop();
    }

    public interface IHaveOptions:IAgent
    {
    }

    public interface IHaveOptions<TOption>: IHaveOptions where TOption:class
    {
        TOption Options { set; }
    }

    public interface IStatefulAgent :IAgent
    {
        IAgentState StateService { get; set; }
    }

    /// <summary>
    /// An agent with with state that can seamlessly be fetched and persisted
    /// </summary>
    public interface IStatefulAgent<TState>:IStatefulAgent, IAgent where TState : class
    {
        TState State { get; set; }
    }

    /// <summary>
    ///     Start *something* on a schedule
    /// </summary>
    public interface IScheduledAgent : IAgent
    {
        int DefaultSchedule { get; }
        void Check();
    }

    /// <summary>
    ///    Publishig agent
    /// </summary>
    public interface IPublishingAgent : IAgent
    {
        IPublisher Publisher { set; }
    }


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
        void Consume(object evt);
    }


    public interface IAgentState
    {
        /// <summary>
        /// As long as the serializers can serialize, you can have state
        /// </summary>
        T GetState<T>();

        /// <summary>
        /// Agent is responsible for persisting it's own state when required
        /// </summary>
        void PersistState(object state);
    }
}
