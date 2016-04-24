using System;

namespace Rabbitual.Agents
{
    public abstract class Agent<TOptions> : IHaveOptions<TOptions> where TOptions : class
    {
        public string Id { get; set; }
        public void Start() { }

        public void Stop() { }

        public TOptions Options { get; set; }
    }

    public abstract class StatefulAgent<TOptions, TState>:Agent<TOptions>, IStatefulAgent<TState> 
        where TOptions : class 
        where TState : class, new()
    {
        public new void Start()
        {
            State = State ?? new TState();
        }

        public new void Stop()
        {
            StateService.PersistState(State);
        }

        public TOptions Options { get; set; }

        public TState State { get; set; }

        public IAgentState StateService { get; set; }
    }

    public abstract class EventConsumerAgent<TOptions> : Agent<TOptions>, IEventConsumerAgent where TOptions : class
    {
        public abstract void Consume(object evt);
    }

    public abstract class ScheduledAgent<TOptions> : Agent<TOptions>, IScheduledAgent where TOptions : class
    {
        public int DefaultSchedule { get; set; }
        public abstract void Check();
    }

    public abstract class ScheduledStatefulAgent<TOptions, TState> :StatefulAgent<TOptions,TState>, IScheduledAgent
        where TOptions : class
        where TState : class, new()
    {
        public int DefaultSchedule { get; set; }
        public abstract void Check();
    }
}