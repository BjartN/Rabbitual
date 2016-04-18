namespace Rabbitual.Agents
{
    public abstract class ScheduledAgent<TOptions> : IScheduledAgent, IHaveOptions<TOptions> where TOptions :class
    {
        public string Id { get; set; }
        public int DefaultSchedule { get; set; }
        public TOptions Options { get; set; }
        public abstract void Check();
    }
}