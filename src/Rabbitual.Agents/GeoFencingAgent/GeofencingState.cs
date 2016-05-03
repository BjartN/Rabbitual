using System;
using System.Collections.Generic;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeofencingState
    {
        public GeofencingState()
        {
            IssuedFences= new List<string>();
        }

        public FenceState FenceState { get; set; }

        public List<string> IssuedFences { get; set; }
    }
}