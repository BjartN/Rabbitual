using Rabbitual.Agents;
using Rabbitual.Agents.WeatherAgent;
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

                var e = new AgentConfig
                {
                    Id = "WeatherAgent 00",
                    Name = "WeatherAgent 00",
                    ClrType = typeof(WeatherAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 0
                    }
                };

                var f = new AgentConfig
                {
                    Id = "WeatherAgent 06",
                    Name = "WeatherAgent 06",
                    ClrType = typeof(WeatherAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 6
                    }
                };

                var g = new AgentConfig
                {
                    Id = "WeatherAgent 12",
                    Name = "WeatherAgent 12",
                    ClrType = typeof(WeatherAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 12
                    }
                };

                var h = new AgentConfig
                {
                    Id = "WeatherAgent 18",
                    Name = "WeatherAgent 18",
                    ClrType = typeof(WeatherAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 18
                    }
                };

                return new[] {e,f,g,h };
            }
        }
    }
}
