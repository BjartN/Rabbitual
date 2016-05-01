using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rabbitual.Agents.GeoFencingAgent;

namespace Rabbitual.UnitTests
{
    [TestFixture]
    public class when_geofencing
    {
        
        private readonly GeofencingOptions _opt = new GeofencingOptions
        {
            ArrivingGrazeTime = TimeSpan.FromMinutes(1),
            LeavingGrazeTime = TimeSpan.FromMinutes(1),
            CircleFences = new[]
                {
                    new Fence
                    {
                        Id = "1",
                        Lat = 0,
                        Lon = 0,
                        RadiusMeters = 1000
                    }
                }
        };

        [Test]
        public void should_issue_fence_breach()
        {
            var now = new DateTime(2000, 1, 1, 12, 0, 0);
            var state = new GeofencingState
            {
                State  =  new Dictionary<string, FenceState>
                {
                    {"1", new FenceState(FenceStateId.Out, now.AddMinutes(-5)) }
                }
            };

            var service = new GeoFencingService(_opt,state,()=> now);

            //move inside and check
            service.MoveTo(0.000001,0.000001);
            Assert.AreEqual(FenceStateId.Arriving,state.State["1"].State);

            //let an hour go by
            now = new DateTime(2000, 1, 1, 13, 0, 0);
            var events = service.TransitionBasedOnTime();

            //..and check that we're in
            Assert.AreEqual(FenceStateId.In, events.Single().Item2.State);

            //let an hour go by
            now = new DateTime(2000, 1, 1, 14, 0, 0);
            events = service.TransitionBasedOnTime();

            //..and check that we're in
            Assert.AreEqual(0, events.Count);

            //move outside and let an hour go by
            service.MoveTo(0.1, 0.1);
            now = new DateTime(2000, 1, 1, 15, 0, 0);
            events = service.TransitionBasedOnTime();

            //..and check that we're outside
            Assert.AreEqual(FenceStateId.Out, events.Single().Item2.State);


        }
    }
}