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
    [Icon("map-marker")]
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

            var service = new GeoFencingService(Options, () => DateTime.UtcNow);
            State.FenceState = service.MoveTo(lat.Value, lon.Value,State.FenceState);
        }

        public void Check()
        {
            if (State.FenceState == null)
                return;

            var service = new GeoFencingService(Options, () => DateTime.UtcNow);
            var newState = service.TransitionBasedOnTime(State.FenceState);
            var change = State.FenceState.Mode != newState.Mode;
            State.FenceState = newState;

            if (!change)
                return;

            var list = new ListManager<FenceState>(State.IssuedFences, limit: 100);
            list.Add(State.FenceState);

            _p.PublishEvent(new Message
            {
                Data = new Dictionary<string, string>
                    {
                        { "fence", Options.CircleFence.Id },
                        { "description",newState.Mode==FenceStateId.In ?  Options.CircleFence.EnteringDescription :  Options.CircleFence.LeavingDescription },
                        { "state", newState.Mode.ToString() }
                    }
            });
        }
    }
}