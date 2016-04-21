using System.ComponentModel;

namespace Rabbitual.Agents.WeatherAgent
{
    public class WeatherOptions
    {
        public WeatherOptions()
        {
            Source = "gfs"; //or ww3
            RunTime = 0;
        }

        [Description("gfs or ww3")]
        public string Source { get; set; }

        [Description("Runtime hour to download")]
        public int RunTime { get; set; }
    }
}