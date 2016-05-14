using System;
using System.Linq;
using NUnit.Framework;
using Rabbitual.Agents.WeatherAgent;

namespace Rabbitual.UnitTests
{
    [TestFixture]
    public class when_parsing_model_url
    {
        [Test]
        public void should_do_it()
        {
            var a = "http://www.ftp.ncep.noaa.gov/data/nccf/com/wave/prod/multi_1.{0:yyyyMMdd}/nww3.tCCz.grib.grib2";
            var b = "http://nomads.ncep.noaa.gov/pub/data/nccf/com/gfs/prod/gfs.{0:yyyyMMddHH}/gfs.t{0:HH}z.pgrb2.1p00.f{2:D3}";

            var url = ModelDataUrlBuilder.GetUrls(a, new DateTime(1978, 11, 16));
            Assert.AreEqual("http://www.ftp.ncep.noaa.gov/data/nccf/com/wave/prod/multi_1.19781116/nww3.tCCz.grib.grib2" ,url.First());


            var u2 = ModelDataUrlBuilder.GetUrls(b, new DateTime(1978, 11, 16), new [] {0,3});
            Assert.AreEqual("http://nomads.ncep.noaa.gov/pub/data/nccf/com/gfs/prod/gfs.1978111600/gfs.t00z.pgrb2.1p00.f000", u2[0]);
            Assert.AreEqual("http://nomads.ncep.noaa.gov/pub/data/nccf/com/gfs/prod/gfs.1978111600/gfs.t00z.pgrb2.1p00.f003", u2[1]);
        }
    }
}