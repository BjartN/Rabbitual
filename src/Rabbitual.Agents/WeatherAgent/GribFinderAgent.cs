using System;
using System.Collections.Generic;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WeatherAgent
{
    /// <summary>
    /// Search the W3 for gribs to download
    /// </summary>
    public class GribFinderAgent :
        ScheduledStatefulAgent<WeatherOptions, GribFinderState>, 
        IPublishingAgent
    {
        private readonly GribSources _d;
        private readonly ILogger _logger;

        public GribFinderAgent(GribSources d, ILogger logger,WeatherOptions options,IAgentStateRepository stateRepository) : base(options,stateRepository)
        {
            _d = d;
            _logger = logger;
            DefaultSchedule = 3000;
        }

        public override void Check()
        {
            var anaTime = getAnaTime();
            var stuffToDownload = Options.Source == "ww3" ? _d.GetWaveWatch3(anaTime) : _d.GetGfs(anaTime);

            foreach (var item in stuffToDownload)
            {
                //don't issue previously issued work
                if (State.Index.ContainsKey(item.Url))
                {
                    continue;
                }
                    
                State.Index[item.Url] = true;
                Publisher.EnqueueTask(new Message
                {
                    Data = new Dictionary<string, string>
                    {
                        {"Url", item.Url },
                        {"File",item.File },
                        {"Folder",Options.Source + "\\" + anaTime.ToString("yyyy-MM-dd-HH-mm") }
                    }
                });
            }

            _logger.Info("Done");
        }

        private DateTime getAnaTime()
        {
            var yesterday = DateTime.UtcNow.AddDays(-1).Date.AddHours(Options.RunTime);
            var today = yesterday.AddDays(1);
            var anaTime = DateTime.UtcNow.Hour >= Options.RunTime
                ? today
                : yesterday;
            return anaTime;
        }

        public IPublisher Publisher { get; set; }
    }

    public class GribFinderState
    {
        public IDictionary<string, bool> Index { get; set; }

        public GribFinderState()
        {
            Index = new Dictionary<string, bool>();
        }
    }
}