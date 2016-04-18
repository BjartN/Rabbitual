using System;
using System.IO;
using System.Linq;

namespace Rabbitual.Agents.WeatherAgent
{
    public class WeatherFolder
    {
        private readonly string _dataDirectory;

        public WeatherFolder(string dataDirectory)
        {
            _dataDirectory = dataDirectory;
        }

        public string GetFolder(string source, DateTime anaTime)
        {
            return Path.Combine(_dataDirectory, source, anaTime.ToString("yyyy-MM-dd-HH-mm"));
        }

        public DateTime[] GetAnatimes()
        {
            var d = DateTime.UtcNow.AddHours(-12);
            var hr = (d.Hour / 6) * 6;
            var anaTime = new DateTime(d.Year, d.Month, d.Day, hr, 0, 0);

            return new[] {anaTime.AddHours(12), anaTime.AddHours(6), anaTime.AddHours(0) };
        }

        public string GetJsonFolder(string source, DateTime anaTime)
        {
            return Path.Combine(_dataDirectory, source + "-json", anaTime.ToString("yyyy-MM-dd-HH-mm"));
        }


        public string GetProtoFolder(string source, DateTime anaTime)
        {
            return Path.Combine(_dataDirectory, source + "-proto", anaTime.ToString("yyyy-MM-dd-HH-mm"));
        }

        public string[] GetSources()
        {
            return new string[] { "ww3", "gfs" };
        }

    }
}