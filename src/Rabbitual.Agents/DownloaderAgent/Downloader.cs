using System;
using System.IO;
using System.Net;
using Rabbitual.Agents.WeatherAgent;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.DownloaderAgent
{
    public  class Downloader
    {
        private readonly ILogger _logger;
        public Downloader(ILogger l)
        {
            _logger = l;
        }

        private Quarantine _q = new Quarantine();
        public void Download(string rootFolder, DownloadItem[] files)
        {
            foreach (var file in files)
            {

                var fullFile = Path.Combine(rootFolder, file.File) + ".tmp";
                var fullFileFinal = Path.Combine(rootFolder, file.File);

                var f = Path.GetDirectoryName(fullFile);
                if (!Directory.Exists(f))
                    Directory.CreateDirectory(f);


                if (File.Exists(fullFileFinal))
                {
                    //_logger.Info(string.Format("File exists {0}", fullFileFinal));
                    continue;
                }

                if (File.Exists(fullFile))
                {
                    try { File.Delete(fullFile); }
                    catch (Exception ex)
                    {
                        _logger.Info("Could not delete .tmp file " + fullFile);
                        continue;
                    }
                }


                if (_q.In(file.Url))
                {
                    _logger.Info("Quarantined " + file.Url);
                    continue;
                }

                var contentLength = HttpContentExtensions.GetContentLength(file.Url);

                if (!contentLength.HasValue)
                {
                    _q.Add(file.Url);
                    _logger.Info("Not there " + file.Url);
                    continue;
                }

                _logger.Info("Downloading \"" + file.Url + "\"...");
                _logger.Info("\tContent length will be " + contentLength);

                try
                {
                    new WebClient().DownloadFile(file.Url, fullFile);
                }
                catch (Exception ex)
                {
                    _logger.Info("Could not download " + file.Url);
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
}