using System;
using System.IO;
using System.Net;
using Rabbitual.Agents.WeatherAgent;
using Rabbitual.Core.Logging;

namespace Rabbitual.Agents.DownloaderAgent
{
    public class DownloadItem
    {
        public string File { get; set; }
        public string Folder { get; set; }
        public string Url { get; set; }
    }

    public interface IDownloader
    {
        void Download(DownloadItem file);
    }

    public class Downloader : IDownloader
    {
        private readonly ILogger _logger;
        public Downloader(ILogger l)
        {
            _logger = l;
        }

        private readonly Quarantine _q = new Quarantine();
        public void Download(DownloadItem file)
        {

            var fullFile = Path.Combine(file.Folder, file.File) + ".tmp";
            var fullFileFinal = Path.Combine(file.Folder, file.File);

            var f = Path.GetDirectoryName(fullFile);
            if (!Directory.Exists(f))
                Directory.CreateDirectory(f);


            if (File.Exists(fullFileFinal))
            {
                return;
            }

            if (File.Exists(fullFile))
            {
                try { File.Delete(fullFile); }
                catch (Exception ex)
                {
                    _logger.Info("Could not delete .tmp file " + fullFile);
                    return;
                }
            }


            if (_q.In(file.Url))
            {
                _logger.Info("Quarantined " + file.Url);
                return;
            }

            var contentLength = HttpContentExtensions.GetContentLength(file.Url);

            if (!contentLength.HasValue)
            {
                _q.Add(file.Url);
                _logger.Info("Not there " + file.Url);
                return;
            }

            _logger.Debug("Downloading \"" + file.Url + "\"...");
            _logger.Info("\tContent length will be " + contentLength);

            try
            {
                new WebClient().DownloadFile(file.Url, fullFile);
            }
            catch (Exception ex)
            {
                _logger.Debug("\tCould not download");
            }

            try
            {
                File.Move(fullFile, fullFileFinal);
            }
            catch
            {
                _logger.Info("Could not rename to file name " + fullFileFinal);
            }

            _logger.Info("Done with " + file.Url);
        }
    }
}