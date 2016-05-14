using System.IO;

namespace Rabbitual.Agents.WeatherAgent
{
    public interface IDiskSize
    {
        long GetTotalFreeSpace(string driveName);
    }

    public class DiskSize : IDiskSize
    {
        public long GetTotalFreeSpace(string driveName)
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
}