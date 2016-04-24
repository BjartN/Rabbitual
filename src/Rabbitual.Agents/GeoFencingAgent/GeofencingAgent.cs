using System.ComponentModel;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeofencingAgent: StatefulAgent<GeofencingOptions,GeofencingState>
        , IEventConsumerAgent
        , IPublishingAgent
    {
        public GeofencingAgent(GeofencingOptions options,  IAgentStateRepository stateRepository) 
            : base(options, stateRepository)
        {
        }

        public void Consume(Message evt)
        {
            var lat = evt.Data.AsDouble("lat");
            var lon = evt.Data.AsDouble("lon");

            if (lat == null || lon == null)
                return;

            foreach (var fence in Options.CircleFences)
            {
                var rRadius = GeometryFun.RadiusDegrees(fence.RadiusMeters,fence.Lat,fence.Lon);
                var isMatch = GeometryFun.IsPointInCircle(fence.Lon, fence.Lat, rRadius, lon.Value, lat.Value);
                if (!isMatch)
                    continue;

                //publish fence breached event
                Publisher.PublishEvent(new Message {Data = {["fence"] = fence.Id}});
            }
        }

        public IPublisher Publisher { get; set; }

    }

    public class GeofencingState
    {
        
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

    }
}