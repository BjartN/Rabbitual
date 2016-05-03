using System;
using System.Collections.Generic;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeoFencingService
    {
        private readonly GeofencingOptions _o;
        private readonly GeofencingState _s;
        private readonly Func<DateTime> _now;

        public GeoFencingService(GeofencingOptions o, GeofencingState s, Func<DateTime> now)
        {
            _o = o;
            _s = s;
            _now = now;
        }

        public FenceState MoveTo(double lat, double lon)
        {
            return moveTo(lat, lon, _o.CircleFence, _s.FenceState);
        }

        public FenceState TransitionBasedOnTime()
        {
            var now = _now();

            if (_s.FenceState.Mode == FenceStateId.Arriving && (now - _s.FenceState.When) > _o.ArrivingGrazeTime)
            {
                //you have now arrived
                return new FenceState(FenceStateId.In, now);
            }

            if (_s.FenceState.Mode == FenceStateId.Leaving && (now - _s.FenceState.When) > _o.LeavingGrazeTime)
            {
                return new FenceState(FenceStateId.Out, now);
            }

            //same as before
            return new FenceState(_s.FenceState.Mode, _s.FenceState.When);
        }

        private FenceState moveTo(double lat, double lon, Fence fence, FenceState fenceState)
        {
            var now = _now();
            var rRadius = GeometryFun.RadiusDegrees(fence.RadiusMeters, fence.Lat, fence.Lon);
            var isMatch = GeometryFun.IsPointInCircle(fence.Lon, fence.Lat, rRadius, lon, lat);


            if (fenceState == null)
            {
                //set initital state
                return new FenceState(isMatch ? FenceStateId.In : FenceStateId.Out, now);
            }

            if (isMatch)
            {
                switch (fenceState.Mode)
                {
                    case FenceStateId.Out:      //out -> in : arriving
                        return new FenceState(FenceStateId.Arriving, now);
                    case FenceStateId.In:       //in -> in : in
                        return new FenceState(FenceStateId.In, now);
                    case FenceStateId.Arriving: //arriving -> in : arriving
                        return new FenceState(FenceStateId.Arriving, fenceState.When);
                    case FenceStateId.Leaving:  //leaving -> in : in
                        return new FenceState(FenceStateId.In, now);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else //outside fence
            {

                switch (fenceState.Mode)
                {
                    case FenceStateId.Out:      //out -> out : out
                        return new FenceState(FenceStateId.Out, now);
                    case FenceStateId.In:       //in -> out : leaving
                        return new FenceState(FenceStateId.Leaving, now);
                    case FenceStateId.Arriving: //arriving -> out : out
                        return new FenceState(FenceStateId.Out, fenceState.When);
                    case FenceStateId.Leaving:  //leaving -> out : leaving
                        return new FenceState(FenceStateId.Leaving, now);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}