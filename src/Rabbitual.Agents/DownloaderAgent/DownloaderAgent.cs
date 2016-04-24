using System;
using System.IO;
using System.Linq;
using Rabbitual.Agents.WeatherAgent;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.DownloaderAgent
{
    public class DownloaderAgent : Agent<DownloaderOptions>, ITaskConsumerAgent
    {
        private readonly Downloader _d;
        private readonly ILogger _logger;

        public DownloaderAgent(Downloader d, ILogger logger,DownloaderOptions options):base(options)
        {
            _d = d;
            _logger = logger;
        }

        public bool CanDoWork(Message task)
        {
            return true;
        }

        public void DoWork(Message task)
        {
            if (task == null || !(task.Data.ContainsKey("Url") && task.Data.ContainsKey("File") && task.Data.ContainsKey("Folder")))
                return;

            if (DiskSize.GetTotalFreeSpace(Options.DataFolderDrive) < Options.MinimumDiskSize)
            {
                _logger.Warn("Disc almost full. Aborting");
                return;
            }

            var di = new DownloadItem(Path.Combine(task.Data["Folder"], task.Data["File"]), task.Data["Url"]);
            _logger.Info("Starting download check for {0} at {1}", task.Data["Url"], DateTime.UtcNow);
            _d.Download(Options.DataFolder,  di);
            _logger.Info("Done");
        }

    }
}