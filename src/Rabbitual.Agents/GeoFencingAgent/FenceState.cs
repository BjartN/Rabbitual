using System;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class FenceState
    {
        public FenceState(FenceStateId mode, DateTime @when)
        {
            Mode = mode;
            When = when;
        }

        public FenceStateId Mode { get; set; }
        public DateTime When { get; set; }
    }
}