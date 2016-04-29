using System;
using System.Collections.Generic;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeoFencingService
    {
        private readonly GeofencingOptions _o;
        private readonly GeofencingState _s;
        private readonly DateTime _now;

        public GeoFencingService(GeofencingOptions o, GeofencingState s, DateTime now)
        {
            _o = o;
            _s = s;
            _now = now;
        }

        public void MoveTo(double lat, double lon)
        {
            foreach (var fence in _o.CircleFences)
            {
                FenceState fenceState;
                _s.State.TryGetValue(fence.Id, out fenceState);
                _s.State[fence.Id] = moveTo(lat, lon, fence, fenceState);
            }
        }

        public List<Tuple<Fence, FenceState>> TransitionBasedOnTime()
        {
            var l = new List<Tuple<Fence, FenceState>>();
            foreach (var fence in _o.CircleFences)
            {
                FenceState fenceState;
                _s.State.TryGetValue(fence.Id, out fenceState);
                if (fenceState == null)
                    continue;

                var newState = TransitionBasedOnTime(fenceState);
                if (newState == null)
                    continue;

                _s.State[fence.Id] = newState;
                l.Add(new Tuple<Fence, FenceState>(fence, newState));
            }
            return l;
        }


        /// <summary>
        ///     If have been in arriving or leaving state in the configured amount of time, issue change
        /// </summary>
        public FenceState TransitionBasedOnTime(FenceState currentState)
        {
            if (currentState.State == FenceStateId.Arriving && (_now - currentState.When) > _o.ArrivingGrazeTime)
            {
                //you have now arrived
                return new FenceState(FenceStateId.In, _now);
            }

            if (currentState.State == FenceStateId.Leaving && (_now - currentState.When) > _o.LeavingGrazeTime)
            {
                return new FenceState(FenceStateId.Out, _now);
            }

            return null;
        }

        private FenceState moveTo(double lat, double lon, Fence fence, FenceState fenceState)
        {
            var rRadius = GeometryFun.RadiusDegrees(fence.RadiusMeters, fence.Lat, fence.Lon);
            var isMatch = GeometryFun.IsPointInCircle(fence.Lon, fence.Lat, rRadius, lon, lat);


            if (fenceState == null)
            {
                //set initital state
                return new FenceState(isMatch ? FenceStateId.In : FenceStateId.Out, _now);
            }

            if (isMatch)
            {
                switch (fenceState.State)
                {
                    case FenceStateId.Out:      //out -> in : arriving
                        return new FenceState(FenceStateId.Arriving, _now);
                    case FenceStateId.In:       //in -> in : in
                        return new FenceState(FenceStateId.In, _now);
                    case FenceStateId.Arriving: //arriving -> in : arriving
                        return new FenceState(FenceStateId.Arriving, fenceState.When);
                    case FenceStateId.Leaving:  //leaving -> in : in
                        return new FenceState(FenceStateId.In, _now);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else //outside fence
            {

                switch (fenceState.State)
                {
                    case FenceStateId.Out:      //out -> out : out
                        return new FenceState(FenceStateId.Out, _now);
                    case FenceStateId.In:       //in -> out : leaving
                        return new FenceState(FenceStateId.Leaving, _now);
                    case FenceStateId.Arriving: //arriving -> out : out
                        return new FenceState(FenceStateId.Out, fenceState.When);
                    case FenceStateId.Leaving:  //leaving -> out : leaving
                        return new FenceState(FenceStateId.Leaving, _now);
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }
}