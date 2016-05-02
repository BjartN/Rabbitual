namespace Rabbitual.Agents
{
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