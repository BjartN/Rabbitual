using System.ComponentModel;

namespace Rabbitual.Agents.DownloaderAgent
{
    public class DownloaderOptions
    {
        public DownloaderOptions()
        {
            DataFolder = @"c:\data\grib-data";
            DataFolderDrive = @"C:\";
            MinimumDiskSize = 1000;
        }

        [Description("Location to store files")]
        public string DataFolder { get; set; }

        [Description("Drive to monitor for fullness. I.e. c:\\")]
        public string DataFolderDrive { get; set; }

      
        [Description("Minimum disk size in Mb. Stop downloading after reaching this.")]
        public int MinimumDiskSize { get; set; }

    }
}