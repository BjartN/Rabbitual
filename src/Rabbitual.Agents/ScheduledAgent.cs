namespace Rabbitual.Agents
{
    public abstract class ScheduledAgent<TOptions> : Agent<TOptions>, IScheduledAgent where TOptions : class
    {
        public int DefaultScheduleMs { get; set; }
        public abstract void Check();

        public ScheduledAgent(TOptions options) : base(options)
        {
        }
    }
}