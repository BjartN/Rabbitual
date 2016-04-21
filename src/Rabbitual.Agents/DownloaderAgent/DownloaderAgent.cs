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
        readonly object _locker = new object();
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

            _logger.Info("Starting download check for {0} at {1}", m.Data["Url"], DateTime.UtcNow);
            var di = new DownloadItem(Path.Combine(m.Data["Folder"], m.Data["File"]), m.Data["Url"]);

            _logger.Debug("I see lock says thread {0}, instance {1}", System.Threading.Thread.CurrentThread.ManagedThreadId, _instanceId);
            lock (_locker)
            {
                _logger.Debug("Inside lock from thread {0}. Downloading", System.Threading.Thread.CurrentThread.ManagedThreadId);
                _d.Download(Options.DataFolder, new[] { di });
            }
            _logger.Info("Done");
        }

        public string Id { get; set; }
        public DownloaderOptions Options { get; set; }
    }
}