using System;
using System.IO;
using System.Linq;
using Rabbitual.Agents.WeatherAgent;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.DownloaderAgent
{
    public class DownloaderAgent : ITaskConsumerAgent, IHaveOptions<DownloaderOptions>
    {
        private readonly Downloader _d;
        private readonly ILogger _logger;
        private static int _globalId = 0;
        private int _instanceId;

        public DownloaderAgent(Downloader d, ILogger logger)
        {
            _d = d;
            _logger = logger;
            _instanceId = _globalId++;
        }

        public bool CanWorkOn(object task)
        {
            return true;
        }

        public void DoWork(object evt)
        {
            var m = evt as Message;
            if (m == null || !(m.Data.ContainsKey("Url") && m.Data.ContainsKey("File") && m.Data.ContainsKey("Folder")))
                return;

            if (DiskSize.GetTotalFreeSpace(Options.DataFolderDrive) < Options.MinimumDiskSize)
            {
                _logger.Warn("Disc almost full. Aborting");
                return;
            }

            var di = new DownloadItem(Path.Combine(m.Data["Folder"], m.Data["File"]), m.Data["Url"]);
            _logger.Info("Starting download check for {0} at {1}", m.Data["Url"], DateTime.UtcNow);
            _d.Download(Options.DataFolder,  di);
            _logger.Info("Done");
        }

        public string Id { get; set; }
        public DownloaderOptions Options { get; set; }
    }
}