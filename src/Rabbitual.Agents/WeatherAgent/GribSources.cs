using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rabbitual.Agents.WeatherAgent
{

    public class DownloadItem
    {
        public DownloadItem(string file, string url)
        {
            File = file;
            Url = url;
        }

        public string File { get; set; }
        public string Url { get; set; }
    }

    public class GribSources
    {
        public DownloadItem[] GetWaveWatch3(DateTime anaTime)
        {
            //Wave watch 3
            //http://www.ftp.ncep.noaa.gov/data/nccf/com/wave/prod/multi_1.20150502/nww3.tCCz.grib.grib2
            //Seems to be a single file per anatime

            const string urlTemplate = @"http://www.ftp.ncep.noaa.gov/data/nccf/com/wave/prod/multi_1.{anaDate}/{file}";
            const string fileTemplate = "nww3.t{anaHour}z.grib.grib2";


            var l = new List<DownloadItem>();


            var file = fileTemplate.Replace("{anaHour}", anaTime.ToString("HH"));

            var url = urlTemplate
                .Replace("{file}", file)
                .Replace("{anaDate}", anaTime.ToString("yyyyMMdd"));

            l.Add(new DownloadItem(file, url));

            return l.ToArray();
        }

        public DownloadItem[] GetGfs(DateTime anaTime)
        {

            var urlTemplate = @"http://nomads.ncep.noaa.gov/pub/data/nccf/com/gfs/prod/gfs.{anaTime}/{file}";
            var fcTimes = new[] { "000", "003", "006", "009", "012", "015", "018", "021", "024", "027", "030", "036", "039", "042", "048", "051", "054", "057", "060", "063", "066", "069", "072" };


            var l = new List<DownloadItem>();

            foreach (var fcTime in fcTimes)
            {
                //var file = "gfs.t{anaHour}z.pgrb2.2p50.f{fcHour}";
                var file = "gfs.t{anaHour}z.pgrb2.1p00.f{fcHour}";

                file = file
                    .Replace("{anaHour}", anaTime.ToString("HH"))
                    .Replace("{fcHour}", fcTime);

                var url = urlTemplate
                    .Replace("{file}", file)
                    .Replace("{anaTime}", anaTime.ToString("yyyyMMddHH"));

                l.Add(new DownloadItem(file, url));
            }
            return l.ToArray();
        }

       
    }


    public static class DiskSize
    {
        public static long GetTotalFreeSpace(string driveName)
        {
            foreach (DriveInfo drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady && drive.Name == driveName)
                {
                    return drive.AvailableFreeSpace / 1000000;
                }
            }
            return -1;
        }
    }

    public static class HttpContentExtensions
    {
        public static int? GetContentLength(string url)
        {
            try
            {
                System.Net.WebRequest req = System.Net.HttpWebRequest.Create(url);
                req.Method = "HEAD";
                using (System.Net.WebResponse resp = req.GetResponse())
                {
                    int contentLength;
                    if (int.TryParse(resp.Headers.Get("Content-Length"), out contentLength))
                    {
                        return contentLength;
                    }
                    return 0; //content length not avail
                }
            }
            catch
            {
                return null; //crash
            }
        }

        public static Task ReadAsFileAsync(this HttpContent content, string filename, bool overwrite)
        {
            var pathname = Path.GetFullPath(filename);
            if (!overwrite && File.Exists(filename))
                return Task.FromResult(0);

            FileStream fileStream = null;
            try
            {
                fileStream = new FileStream(pathname, FileMode.Create, FileAccess.Write, FileShare.None);
                return content
                    .CopyToAsync(fileStream)
                    .ContinueWith(copyTask => fileStream.Close());
            }
            catch
            {
                if (fileStream != null)
                    fileStream.Close();
                throw;
            }
        }
    }

    public class Quarantine
    {
        IDictionary<string, DateTime> _d = new Dictionary<string, DateTime>();

        public void Add(string url)
        {
            _d[url] = DateTime.UtcNow;
        }

        public bool In(string url)
        {
            var inQ = _d.ContainsKey(url) && _d[url] < DateTime.UtcNow.AddMinutes(5);

            return inQ;
        }

    }
}