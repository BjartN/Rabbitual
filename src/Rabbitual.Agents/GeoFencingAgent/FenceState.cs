using System;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class FenceState
    {
        public FenceState(FenceStateId state, DateTime @when)
        {
            State = state;
            When = when;
        }

        public FenceStateId State { get; set; }
        public DateTime When { get; set; }
    }
}