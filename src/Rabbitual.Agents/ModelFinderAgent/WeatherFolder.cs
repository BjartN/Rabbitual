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