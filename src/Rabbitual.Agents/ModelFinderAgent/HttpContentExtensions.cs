using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Rabbitual.Agents.WeatherAgent
{
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
}