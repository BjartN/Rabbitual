using System;
using System.Collections.Generic;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeofencingState
    {
        public GeofencingState()
        {
            IssuedFences= new List<string>();
            State= new Dictionary<string, FenceState>();
        }

        public IDictionary<string, FenceState> State { get; set; }

        public List<string> IssuedFences { get; set; }
    }
}