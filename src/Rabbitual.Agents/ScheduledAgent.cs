namespace Rabbitual.Agents
{
    public abstract class Agent<TOptions> : IHaveOptions<TOptions> where TOptions : class
    {
        public string Id { get; set; }
        public TOptions Options { get; set; }
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
}