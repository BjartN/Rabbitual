using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using Rabbitual.Agents;
using Rabbitual.Agents.DownloaderAgent;
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

                var urls = new[]
                {
                    "http://ba.no",
                    "http://bt.no",
                    "http://dn.no",
                    "http://ap.no",
                    "http://sysla.no",
                    "http://db.no"
                };

                var cc = urls.Select(url => new AgentConfig
                {
                    Id = "WebCheckerAgent " + url,
                    Name = "WebCheckerAgent",
                    ClrType = typeof (WebCheckerAgent),
                    Options = new WebCheckerOptions
                    {
                        Url = url
                    }
                }).ToArray();

                var d = new AgentConfig
                {
                    Id = "StatsAgent",
                    Name = "StatsAgent",
                    ClrType = typeof(StatsAgent),
                    Sources = cc 
                };

                var e = new AgentConfig
                {
                    Id = "WeatherAgent 00",
                    Name = "WeatherAgent 00",
                    ClrType = typeof(GribFinderAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 0
                    }
                };

                var f = new AgentConfig
                {
                    Id = "WeatherAgent 06",
                    Name = "WeatherAgent 06",
                    ClrType = typeof(GribFinderAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 6
                    }
                };

                var g = new AgentConfig
                {
                    Id = "WeatherAgent 12",
                    Name = "WeatherAgent 12",
                    ClrType = typeof(GribFinderAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 12
                    }
                };

                var h = new AgentConfig
                {
                    Id = "WeatherAgent 18",
                    Name = "WeatherAgent 18",
                    ClrType = typeof(GribFinderAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 18
                    }
                };

                var i = new AgentConfig
                {
                    Id = "DownloaderAgent 007",
                    Name = "DownloaderAgent 007",
                    ClrType = typeof(DownloaderAgent),
                    Options = new DownloaderOptions(),
                    Sources = new[] {g}
                };


                var i2 = new AgentConfig
                {
                    Id = "DownloaderAgent 008",
                    Name = "DownloaderAgent 008",
                    ClrType = typeof(DownloaderAgent),
                    Options = new DownloaderOptions(),
                    Sources = new[] { g }
                };


                var i3 = new AgentConfig
                {
                    Id = "DownloaderAgent 009",
                    Name = "DownloaderAgent 009",
                    ClrType = typeof(DownloaderAgent),
                    Options = new DownloaderOptions(),
                    Sources = new[] { g }
                };

                var l = new List<AgentConfig> {d,e,f,g,h,i,i2,i3 };
                l.AddRange(cc);

                return l.ToArray();
            }
        }
    }
}
