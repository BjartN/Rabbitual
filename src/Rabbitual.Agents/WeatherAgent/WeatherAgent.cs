using System;
using System.Linq;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WeatherAgent
{
    public class WeatherAgent : ScheduledAgent<WeatherOptions>
    {
        private readonly GribDownloader _d;
        private readonly ILogger _logger;

        public WeatherAgent(GribDownloader d, ILogger logger)
        {
            _d = d;
            _logger = logger;
            DefaultSchedule = 10000;
        }

        public override void Check()
        {
            if (DiskSize.GetTotalFreeSpace(Options.DataFolderDrive) < Options.MinimumDiskSize)
            {
                _logger.Log("Disc almost full. Aborting");
                return;
            }

            _logger.Log("Starting download check at " + DateTime.UtcNow);
            var folders = new WeatherFolder(Options.DataFolder);

            if (!new[] { "gfs", "ww3" }.Contains(Options.Source))
                return;

            var yesterday = DateTime.UtcNow.AddDays(-1).Date.AddHours(Options.RunTime);
            var today = yesterday.AddDays(1);
            var anaTime = DateTime.UtcNow.Hour >= Options.RunTime
                ? today
                : yesterday;

            _d.Download(folders.GetFolder(Options.Source, anaTime), Options.Source == "ww3" ? _d.GetWaveWatch3(anaTime) : _d.GetGfs(anaTime));

            _logger.Log("Done");
        }
    }
}