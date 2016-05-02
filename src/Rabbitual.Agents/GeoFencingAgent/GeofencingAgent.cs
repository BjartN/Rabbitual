using System;
using System.Collections.Generic;
using System.ComponentModel;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.GeoFencingAgent
{

    public enum FenceStateId
    {
        Leaving, Arriving, In, Out
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
    public class GeofencingAgent : StatefulAgent<GeofencingOptions, GeofencingState>
        , IEventConsumerAgent
        , IEventPublisherAgent
        , IScheduledAgent
    {
        public int DefaultScheduleMs => 10000;
        private readonly IMessagePublisher _p;

        public GeofencingAgent(GeofencingOptions options, IAgentStateRepository asr, IMessagePublisher p)
            : base(options, asr)
        {
            _p = p;
        }

        public void Consume(Message evt)
        {
            var lat = evt.Data.TryGetDouble("lat");
            var lon = evt.Data.TryGetDouble("lon");

            if (lat == null || lon == null)
                return;

            var service = new GeoFencingService(Options, State,()=> DateTime.UtcNow);
            service.MoveTo(lat.Value, lon.Value);
        }

        public void Check()
        {
            var service = new GeoFencingService(Options, State, () => DateTime.UtcNow);
            var events = service.TransitionBasedOnTime();
            var list = new ListManager<string>(State.IssuedFences, limit: 100);

            foreach (var e in events)
            {
                list.Add(e.Item1.Id);
                _p.PublishEvent(new Message
                {
                    Data = new Dictionary<string, string>
                    {
                        { "fence", e.Item1.Id },
                        { "description",e.Item2.State==FenceStateId.In ?  e.Item1.EnteringDescription :  e.Item1.LeavingDescription },
                        { "state", e.Item2.State.ToString() }
                    }
                });
            }
        }
    }
}