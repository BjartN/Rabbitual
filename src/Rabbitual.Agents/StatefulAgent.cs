namespace Rabbitual.Agents
{
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
}