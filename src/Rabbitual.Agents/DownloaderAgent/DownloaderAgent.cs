using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Rabbitual.Agents.WeatherAgent;
using Rabbitual.Core;
using Rabbitual.Core.Logging;

namespace Rabbitual.Agents.DownloaderAgent
{
    /// <summary>
    /// Dowloads urls when recieiving task
    /// </summary>
    public class DownloaderAgent : Agent<DownloaderOptions>
        , ITaskConsumerAgent
        , IEventPublisherAgent
    {
        private readonly IMessagePublisher _p;
        private readonly IDownloader _d;
        private readonly ILogger _logger;
        private readonly IDiskSize _disk;

        public DownloaderAgent(
            IMessagePublisher p,
            IDownloader d, 
            ILogger logger, 
            IDiskSize disk,
            DownloaderOptions options):base(options)
        {
            _p = p;
            _d = d;
            _logger = logger;
            _disk = disk;
        }

        public bool CanDoWork(Message task)
        {
            return true;
        }

        public void DoWork(Message task)
        {
            if (task == null || ! task.Data.ContainsKey("Url"))
                return;

            if (_disk.GetTotalFreeSpace(Options.DataFolderDrive) < Options.MinimumDiskSize)
            {
                _logger.Warn("Disc almost full. Aborting");
                return;
            }

            var di = getItemToDownload(task);

            _logger.Info("Starting download check for {0} at {1}", di.Url, DateTime.UtcNow);
            _d.Download(di);

            _p.PublishEvent(new Message
            {
                Data = new Dictionary<string, string>
                {
                    {"Url", di.Url },
                    {"FilePath", Path.Combine(di.Folder,di.File)},
                }
            });

            _logger.Info("Done");
        }

        private DownloadItem getItemToDownload(Message task)
        {
            var url = new Uri(task.Data["Url"]);
            var path = url.LocalPath.Trim(' ', '/').Split('/').ToList();
            path.Insert(0, url.Host);
            path.Insert(0, Options.DataFolder);
            var file = path.Last();
            path.Remove(file);
            var folder = Path.Combine(path.ToArray());
            var di = new DownloadItem
            {
                Url = task.Data["Url"],
                File = file,
                Folder = folder
            };
            return di;
        }
    }
}