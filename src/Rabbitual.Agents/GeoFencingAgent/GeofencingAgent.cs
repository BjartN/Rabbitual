using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.GeoFencingAgent
{

    public enum FenceState
    {
        Leaving, Arriving, In, Out
    }

    public class GeoFencingService
    {
        private readonly GeofencingOptions _o;
        private readonly GeofencingState _s;

        public GeoFencingService(GeofencingOptions o, GeofencingState s)
        {
            _o = o;
            _s = s;
        }

        public void EvaluateEvent(double lat, double lon, DateTime occurred)
        {
            foreach (var fence in _o.CircleFences)
            {
                if (_s.IssuedFences.Any() && _s.IssuedFences.Last() == fence.Id)
                    continue; //don't issue same fence again

                var rRadius = GeometryFun.RadiusDegrees(fence.RadiusMeters, fence.Lat, fence.Lon);
                var isMatch = GeometryFun.IsPointInCircle(fence.Lon, fence.Lat, rRadius, lon, lat);
         

            }

        }
    }



    [Description(@"
        Possible states, decided by setting
            leaving-graze-time
            arriving-graze-time

        in -> leaving -> out
        in -> leaving -> in
        out -> arriving -> in
        out -> arriving -> out
    ")]
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
            State= new Dictionary<string, Tuple<FenceState, DateTime>>();
        }

        public IDictionary<string,Tuple<FenceState, DateTime>> State { get; set; }

        public List<string> IssuedFences { get; set; }
    }

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

    public class Fence
    {
        public double Lat { get; set; }
        public double Lon { get; set; }

        public int RadiusMeters { get; set; }
        public string  Id { get; set; }

        public string Description { get; set; }

    }
}