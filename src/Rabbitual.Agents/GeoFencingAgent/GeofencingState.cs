using System;
using System.Collections.Generic;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeofencingState
    {
        public GeofencingState()
        {
            IssuedFences = new List<FenceState>();
        }

        public FenceState FenceState { get; set; }

        public List<FenceState> IssuedFences { get; set; }
    }
}