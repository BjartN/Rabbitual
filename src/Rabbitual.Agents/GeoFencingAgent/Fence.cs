namespace Rabbitual.Agents.GeoFencingAgent
{
    public class Fence
    {
        public double Lat { get; set; }
        public double Lon { get; set; }

        public int RadiusMeters { get; set; }
        public string  Id { get; set; }

        public string LeavingDescription { get; set; }

        public string EnteringDescription { get; set; }

    }
}