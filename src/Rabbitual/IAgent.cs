using System.Collections.Generic;
using System.Security.AccessControl;

namespace Rabbitual
{
    /// <summary>
    /// An agent can perform tasks, recieve events or do something on a schedule.
    /// </summary>
    public interface IAgent
    {

    }

    public interface IOptionsAgent: IAgent
    {
         IDictionary<string,string> Options { get;  }
    }

    /// <summary>
    /// An agent with with state that can seamlessly be fetched and persisted
    /// </summary>
    public interface IStatefulAgent:IAgent
    {
        void Start(IAgentState ctx);

        /// <summary>
        /// Giving the agent a chance to persist its changes
        /// </summary>
        void Stop();
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
    public interface IEventConsumerAgent : IAgent
    {
        bool CanConsume(object evt);
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
