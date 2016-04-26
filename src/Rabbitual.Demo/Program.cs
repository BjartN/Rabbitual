using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Rabbitual.Agents;
using Rabbitual.Agents.CsvAgent;
using Rabbitual.Agents.DownloaderAgent;
using Rabbitual.Agents.EmailAgent;
using Rabbitual.Agents.GeoFencingAgent;
using Rabbitual.Agents.StatsAgent;
using Rabbitual.Agents.TextAgent;
using Rabbitual.Agents.UniqueEventAgent;
using Rabbitual.Agents.WeatherAgent;
using Rabbitual.Agents.WebCheckerAgent;
using Rabbitual.Agents.WebServerAgent;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

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
                return getGeofenceScenario();
            }

            private AgentConfig[] getGeofenceScenario()
            {
                var a = new AgentConfig
                {
                    Id = "CsvAgent",
                    Name = "CsvAgent",
                    ClrType = typeof(CsvAgent),
                    Options = new CsvOptions
                    {
                        FieldNamesAtFirstLine = false,
                        FieldNames = new[] { "lat", "lon", "", "", "", "", "", "time" },
                        StartAtEnd = true,
                        RowCount = 10,
                        Separator = "|",
                        Url = ConfigurationManager.AppSettings["csv-url"]
                    }
                };

                var b = new AgentConfig
                {
                    Id = "UniqueEvent",
                    Name = "UniqueEvent",
                    ClrType = typeof(UniqueEventAgent),
                    Options = new UniqueOptions
                    {
                        KeepStateMinutes = 60 * 24,
                        IdField = "time"
                    },
                    Sources = new[] { a }
                };

                var c = new AgentConfig
                {
                    Id = "GeoFence",
                    Name = "GeoFence",
                    ClrType = typeof(GeofencingAgent),
                    Options = new GeofencingOptions
                    {
                        CircleFences = new[] {
                            new Fence
                            {
                                Id = "Home",
                                Description = "I'm at home",
                                Lat =ConfigurationManager.AppSettings["home-lat"].ToDouble(),
                                Lon =ConfigurationManager.AppSettings["home-lon"].ToDouble(),
                                RadiusMeters = 1000
                            }
                        }
                    },
                    Sources = new[] { b }

                };

                var d = new AgentConfig
                {
                    Id = "Text",
                    Name = "Text",
                    ClrType = typeof(TextAgent),
                    Options = new TextOptions
                    {
                        Template = "Fence {fence} is breached: {description}"
                    },
                    Sources = new[] { c },
                };

                var e = new AgentConfig
                {
                    Id = "Email",
                    Name = "Email",
                    ClrType = typeof(EmailAgent),
                    Options = new EmailOptions
                    {
                        BodyTemplate = "{text}",
                        FromEmail = ConfigurationManager.AppSettings["from-email"],
                        ToEmail = ConfigurationManager.AppSettings["to-email"],
                        MaxEmailCountPerHour = 10,
                        SubjectTemplate = "Geofence Event!"
                    },
                    Sources = new[] { d }
                };


                var webServer = new AgentConfig
                {
                    Id = "WebServer",
                    Name = "WebServer",
                    ClrType = typeof(WebServerAgent),
                };

                return new List<AgentConfig> { a, b, c, d, e, webServer }.ToArray();
            }


            private static AgentConfig[] getRandomScenario()
            {
                var b = new AgentConfig
                {
                    Id = "ScheduledPublisherAgent",
                    Name = "ScheduledPublisherAgent",
                    ClrType = typeof(ScheduledPublisherAgent),
                };

                var urls = new[]
                {
                    "http://bt.no",
                    "http://ba.no",
                    "http://dn.no",
                    "http://ap.no",
                    "http://sysla.no",
                    "http://db.no"
                };

                var cc = urls.Select((url, idx) => new AgentConfig
                {
                    Id = "WebCheckerAgent." + idx,
                    Name = "WebCheckerAgent",
                    ClrType = typeof(WebCheckerAgent),
                    Options = new WebCheckerOptions
                    {
                        Url = url
                    }
                }).Take(1).ToArray();

                var d = new AgentConfig
                {
                    Id = "StatsAgent",
                    Name = "StatsAgent",
                    ClrType = typeof(StatsAgent),
                    Sources = cc
                };

                var e = new AgentConfig
                {
                    Id = "WeatherAgent.00",
                    Name = "WeatherAgent 00",
                    ClrType = typeof(GribFinderAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 0
                    }
                };

                var f = new AgentConfig
                {
                    Id = "WeatherAgent.06",
                    Name = "WeatherAgent 06",
                    ClrType = typeof(GribFinderAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 6
                    }
                };

                var g = new AgentConfig
                {
                    Id = "WeatherAgent.12",
                    Name = "WeatherAgent 12",
                    ClrType = typeof(GribFinderAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 12
                    }
                };

                var h = new AgentConfig
                {
                    Id = "WeatherAgent.18",
                    Name = "WeatherAgent 18",
                    ClrType = typeof(GribFinderAgent),
                    Options = new WeatherOptions
                    {
                        RunTime = 18
                    }
                };

                var i = new AgentConfig
                {
                    Id = "DownloaderAgent.007",
                    Name = "DownloaderAgent 007",
                    ClrType = typeof(DownloaderAgent),
                    Options = new DownloaderOptions(),
                    Sources = new[] { g }
                };


                var i2 = new AgentConfig
                {
                    Id = "DownloaderAgent.008",
                    Name = "DownloaderAgent 008",
                    ClrType = typeof(DownloaderAgent),
                    Options = new DownloaderOptions(),
                    Sources = new[] { g }
                };


                var i3 = new AgentConfig
                {
                    Id = "DownloaderAgent.009",
                    Name = "DownloaderAgent 009",
                    ClrType = typeof(DownloaderAgent),
                    Options = new DownloaderOptions(),
                    Sources = new[] { g }
                };

                var webServer = new AgentConfig
                {
                    Id = "WebServer",
                    Name = "WebServer",
                    ClrType = typeof(WebServerAgent),
                };

                var l = new List<AgentConfig> { d, e, f, g, h, i, i2, i3, webServer };
                l.AddRange(cc);

                return l.ToArray();
            }

        }

    }
}
