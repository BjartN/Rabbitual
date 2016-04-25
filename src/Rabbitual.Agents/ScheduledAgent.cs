using System;

namespace Rabbitual.Agents
{
    public abstract class Agent<TOptions> : IHaveOptions<TOptions> where TOptions : class
    {
        protected Agent(TOptions options)
        {
            Options = options;
        }

        public string Id { get; set; }
        public virtual void Start() { }
        public virtual void Stop() { }

        public TOptions Options { get; protected set; }
    }

    public abstract class StatefulAgent<TOptions, TState>:Agent<TOptions>, IStatefulAgent<TState> 
        where TOptions : class 
        where TState : class, new()
    {

        public StatefulAgent(TOptions options, IAgentStateRepository stateRepository) : base(options)
        {
            StateRepository = stateRepository;
            State = stateRepository.GetState<TState>() ?? new TState(); ;
        }


        public new void Start()
        {
        }

        public new void Stop()
        {
            StateRepository.PersistState(State);
        }

        public TState State { get; set; }

        public IAgentStateRepository StateRepository { get; set; }

    }

    public abstract class EventConsumerAgent<TOptions> : Agent<TOptions>, IEventConsumerAgent where TOptions : class
    {
        public abstract void Consume(Message evt);

        public EventConsumerAgent(TOptions options) : base(options)
        {
        }
    }

    public abstract class ScheduledAgent<TOptions> : Agent<TOptions>, IScheduledAgent where TOptions : class
    {
        public int DefaultScheduleMs { get; set; }
        public abstract void Check();

        public ScheduledAgent(TOptions options) : base(options)
        {
        }
    }

    public abstract class ScheduledStatefulAgent<TOptions, TState> :StatefulAgent<TOptions,TState>, IScheduledAgent
        where TOptions : class
        where TState : class, new()
    {
        public int DefaultScheduleMs { get; set; }
        public abstract void Check();

        public ScheduledStatefulAgent(TOptions options, IAgentStateRepository stateRepository) : base(options, stateRepository)
        {
        }

    }
}