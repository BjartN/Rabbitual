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
            CircleFence =
                    new Fence
                    {
                        Id = "1",
                        Lat = 0,
                        Lon = 0,
                        RadiusMeters = 1000
                    }
        };

        [Test]
        public void should_issue_fence_breach()
        {
            var now = new DateTime(2000, 1, 1, 12, 0, 0);
            var state = new GeofencingState
            {
                FenceState  =  new FenceState(FenceStateId.Out, now.AddMinutes(-5))
            };

            var service = new GeoFencingService(_opt,state,()=> now);

            //move inside 
            state.FenceState= service.MoveTo(0.000001,0.000001);

            //and check that we are arriving
            Assert.AreEqual(FenceStateId.Arriving,state.FenceState.Mode);

            //let an hour go by
            now = new DateTime(2000, 1, 1, 13, 0, 0);
            state.FenceState = service.TransitionBasedOnTime();

            //..and check that we're in
            Assert.AreEqual(FenceStateId.In, state.FenceState.Mode);

            //let an hour go by
            now = new DateTime(2000, 1, 1, 14, 0, 0);
            var oldState = state.FenceState.Mode;
            state.FenceState = service.TransitionBasedOnTime();

            //..and check that we're still in
            Assert.AreEqual(oldState,state.FenceState.Mode);

            //move outside 
            state.FenceState=service.MoveTo(0.1, 0.1);
            
            //and check that we are leaving
            Assert.AreEqual(FenceStateId.Leaving, state.FenceState.Mode);

            //let an hour go by
            now = new DateTime(2000, 1, 1, 15, 0, 0);
            state.FenceState  = service.TransitionBasedOnTime();

            //..and check that we're outside
            Assert.AreEqual(FenceStateId.Out, state.FenceState.Mode);

        }
    }
}