using System;
using System.Linq;

namespace Rabbitual.Agents.WeatherAgent
{
    /// <summary>
    ///     Format url using anatime and forecast time.
    /// 
    ///     {0} anatime as date, {1} is fctime as timespan, {2} is total fchours
    /// </summary>
    public class ModelDataUrlBuilder
    {
        public const string WW3 = "http://www.ftp.ncep.noaa.gov/data/nccf/com/wave/prod/multi_1.{0:yyyyMMdd}/nww3.tCCz.grib.grib2";
        public const string GFS = "http://nomads.ncep.noaa.gov/pub/data/nccf/com/gfs/prod/gfs.{0:yyyyMMddHH}/gfs.t{0:HH}z.pgrb2.1p00.f{2:D3}";

        public static string[] GetUrls(string urlTemplate, DateTime anaTime, int[] fctimes)
        {
            return GetUrls(urlTemplate,anaTime,fctimes.Select(x=>TimeSpan.FromHours(x)).ToArray());
        }

        public static string[] GetUrls(string urlTemplate, DateTime anaTime, TimeSpan[] fctimes=null)
        {
            if (fctimes == null)
                return new[] {string.Format(urlTemplate, anaTime)};

            return fctimes
                .Select(fc => string.Format(urlTemplate, anaTime, fc,(int)fc.TotalHours))
                .Distinct()
                .ToArray();
        }

    }
}