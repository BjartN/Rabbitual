using System;

namespace Rabbitual.Agents.GeoFencingAgent
{
    public class GeometryFun
    {
        public static bool IsInRectangle(double centerX, double centerY, double radius,double x, double y)
        {
            return x >= centerX - radius && x <= centerX + radius &&
                y >= centerY - radius && y <= centerY + radius;
        }

        //test if coordinate (x, y) is within a radius from coordinate (center_x, center_y)
        public static bool IsPointInCircle(double centerX, double centerY, double radius, double x, double y)
        {
            if (IsInRectangle(centerX, centerY, radius, x, y))
            {
                double dx = centerX - x;
                double dy = centerY - y;
                dx *= dx;
                dy *= dy;
                double distanceSquared = dx + dy;
                double radiusSquared = radius * radius;
                return distanceSquared <= radiusSquared;
            }
            return false;
        }


        public static double RadiusDegrees(int offsetMeters, double lat, double lon)
        {
            //Earth’s radius, sphere
            var R = 6378137;

            //offsets in meters
            var dn = offsetMeters;
            var de = offsetMeters;

            //Coordinate offsets in radians
            var dLat = dn / R;
            var dLon = de / (R * Math.Cos(Math.PI * lat / 180));

            return Math.Max(dLat * 180 / Math.PI, dLon * 180 / Math.PI);
        }
    }
}