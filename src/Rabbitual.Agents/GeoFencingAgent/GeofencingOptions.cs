using System;
using System.ComponentModel;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeofencingOptions
    {
        public GeofencingOptions()
        {
            CircleFence = new Fence();
            LeavingGrazeTime = TimeSpan.FromMinutes(1);
            ArrivingGrazeTime = TimeSpan.FromMinutes(1);
        }

        public TimeSpan ArrivingGrazeTime { get; set; }

        public TimeSpan LeavingGrazeTime { get; set; }

        [Description("State with a radius in meters and a lat/lon center")]
        public Fence CircleFence { get; set; }
    }
}