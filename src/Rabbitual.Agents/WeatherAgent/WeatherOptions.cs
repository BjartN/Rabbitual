using System.ComponentModel;

namespace Rabbitual.Agents.WeatherAgent
{
    public class WeatherOptions
    {
        public WeatherOptions()
        {
            DataFolder = @"c:\data\grib-data";
            DataFolderDrive = @"C:\";
            Source = "gfs"; //or ww3
            MinimumDiskSize = 1000;
            RunTime = 0;
        }

        [Description("Location to store files")]
        public string DataFolder { get; set; }

        [Description("Drive to monitor for fullness. I.e. c:\\")]
        public string DataFolderDrive { get; set; }

        [Description("gfs or ww3")]
        public string Source { get; set; }

        [Description("Minimum disk size in Mb. Stop downloading after reaching this.")]
        public int MinimumDiskSize { get; set; }

        [Description("Runtime hour to download")]
        public int RunTime { get; set; }
    }
}