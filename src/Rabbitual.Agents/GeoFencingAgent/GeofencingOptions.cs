using System;
using System.ComponentModel;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeofencingOptions
    {
        public GeofencingOptions()
        {
            CircleFences = new Fence[0];
            LeavingGrazeTime = TimeSpan.FromMinutes(1);
            ArrivingGrazeTime = TimeSpan.FromMinutes(1);
        }

        public TimeSpan ArrivingGrazeTime { get; set; }

        public TimeSpan LeavingGrazeTime { get; set; }

        [Description("List of lists of [lat,lon,radiusMeter] to describe the circular fence")]
        public Fence[] CircleFences { get; set; }
    }
}