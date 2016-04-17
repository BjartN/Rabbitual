using Rabbitual.Agents;
using Rabbitual.Configuration;

namespace Rabbitual.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            ConsoleHost.Host.Run(true, new AgentConfiguration());
        }

        //TODO: This whole config object should obviously be read as json or similar from some persisted medium and then created like this.
        public class AgentConfiguration : IAgentConfiguration
        {
            public AgentConfig[] GetConfiguration()
            {
                var a = new AgentConfig
                {
                    Id = "CounterAgent",
                    Name = "CounterAgent",
                    ClrType = typeof(CounterAgent),
                };

                var b = new AgentConfig
                {
                    Id = "ScheduledPublisherAgent",
                    Name = "ScheduledPublisherAgent",
                    ClrType = typeof(ScheduledPublisherAgent),
                };

                var c = new AgentConfig
                {
                    Id = "WebCheckerAgent",
                    Name = "WebCheckerAgent",
                    ClrType = typeof(WebCheckerAgent),
                };

                var d = new AgentConfig
                {
                    Id = "StatsAgent",
                    Name = "StatsAgent",
                    ClrType = typeof(StatsAgent),
                    Sources = new[] { c }
                };

                return new[] { a, b, c, d };
            }
        }
    }
}
