using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeofencingAgent: StatefulAgent<GeofencingOptions, GeofencingState>
        , IEventConsumerAgent
        , IEventPublisherAgent
    {
        private readonly IPublisher _p;

        public GeofencingAgent(GeofencingOptions options,IAgentStateRepository asr, IPublisher p) 
            : base(options, asr)
        {
            _p = p;
        }

        public void Consume(Message evt)
        {
            var list = new ListManager<string>(State.IssuedFences, limit:100);
            var lat = evt.Data.TryGetDouble("lat");
            var lon = evt.Data.TryGetDouble("lon");

            if (lat == null || lon == null)
                return;

            foreach (var fence in Options.CircleFences)
            {
                if (State.IssuedFences.Any() && State.IssuedFences.Last() == fence.Id)
                    continue; //don't issue same fence again

                var rRadius = GeometryFun.RadiusDegrees(fence.RadiusMeters,fence.Lat,fence.Lon);
                var isMatch = GeometryFun.IsPointInCircle(fence.Lon, fence.Lat, rRadius, lon.Value, lat.Value);
                if (!isMatch)
                    continue;

                //publish fence breached event
                _p.PublishEvent(new Message {Data = {["fence"] = fence.Id,["description"] = fence.Description } });


                list.Add(fence.Id);
            }
        }

    }

    public class GeofencingState
    {
        public GeofencingState()
        {
            IssuedFences= new List<string>();
        }

        public List<string> IssuedFences { get; set; }
    }

    public class GeofencingOptions
    {
        public GeofencingOptions()
        {
            CircleFences = new Fence[0];
        }

        [Description("List of lists of [lat,lon,radiusMeter] to describe the circular fence")]
        public Fence[] CircleFences { get; set; }
    }

    public class Fence
    {
        public double Lat { get; set; }
        public double Lon { get; set; }

        public int RadiusMeters { get; set; }
        public string  Id { get; set; }

        public string Description { get; set; }

    }
}