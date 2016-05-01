using System;
using System.Collections.Generic;
using NUnit.Framework;
using Rabbitual.Agents.GeoFencingAgent;

namespace Rabbitual.UnitTests
{
    [TestFixture]
    public class when_geofencing
    {
        [Test]
        public void should_issue_fence_breach()
        {
            var now = new DateTime(2000, 1, 1, 12, 0, 0);

            var opt = new GeofencingOptions
            {
                ArrivingGrazeTime = TimeSpan.FromMinutes(1),
                LeavingGrazeTime = TimeSpan.FromMinutes(1),
                CircleFences = new []
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

            var state = new GeofencingState
            {
                State    =  new Dictionary<string, FenceState>
                {
                    {"1", new FenceState(FenceStateId.Out, now.AddMinutes(-5)) }
                }
            };

            var service = new GeoFencingService(opt,state,now);
            service.MoveTo(0.000001,0.000001);
            
            Assert.AreEqual(FenceStateId.Arriving,state.State["1"].State);
        }    
    }
}