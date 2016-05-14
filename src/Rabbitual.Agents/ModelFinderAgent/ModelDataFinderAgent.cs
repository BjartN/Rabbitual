using System;
using System.Collections.Generic;
using Rabbitual.Agents.WeatherAgent;
using Rabbitual.Core;
using Rabbitual.Core.Logging;

namespace Rabbitual.Agents.ModelFinderAgent
{
    /// <summary>
    /// Search the W3 for gribs to download
    /// </summary>
    public class ModelDataFinderAgent :
        ScheduledStatefulAgent<ModelDataFinderOptions, ModelDataFinderState>,
        IEventPublisherAgent
    {
        private readonly ILogger _logger;
        private readonly IMessagePublisher _p;

        public ModelDataFinderAgent(
            ILogger logger,
            ModelDataFinderOptions options,
            IAgentStateRepository stateRepository,
            IMessagePublisher p) : base(options, stateRepository)
        {
            _logger = logger;
            _p = p;
            DefaultScheduleMs = 3000;
        }

        public override void Check()
        {
            var anaTime = getAnaTime();
            string[] urls;
            if (string.IsNullOrWhiteSpace(Options.Source))
                return;

            switch (Options.Source.ToLower())
            {
                case "ww3":
                    urls = ModelDataUrlBuilder.GetUrls(ModelDataUrlBuilder.WW3, anaTime);
                    break;
                case "gfs":
                    urls = ModelDataUrlBuilder.GetUrls(ModelDataUrlBuilder.GFS, anaTime, Options.FcHours);
                    break;
                default:
                    urls = ModelDataUrlBuilder.GetUrls(Options.Source, anaTime, Options.FcHours);
                    break;
            }

            foreach (var item in urls)
            {
                //don't issue previously issued work
                if (State.Index.ContainsKey(item))
                {
                    continue;
                }

                State.Index[item] = true;
                _p.EnqueueTask(new Message
                {
                    Data = new Dictionary<string, string>
                    {
                        {"Url", item},
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

    }

    public class ModelDataFinderState
    {
        public IDictionary<string, bool> Index { get; set; }

        public ModelDataFinderState()
        {
            Index = new Dictionary<string, bool>();
        }
    }
}