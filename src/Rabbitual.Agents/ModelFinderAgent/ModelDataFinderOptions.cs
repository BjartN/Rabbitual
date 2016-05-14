using System.ComponentModel;

namespace Rabbitual.Agents.WeatherAgent
{
    public class ModelDataFinderOptions
    {
        public ModelDataFinderOptions()
        {
            Source = "gfs"; //or ww3
            RunTime = 0;
            FcHours = new[] {
                000, 003, 006, 009, 012, 015, 018, 021,
                024, 027, 030, 036, 039, 042, 048,
                051, 054, 057, 060, 063, 066, 069, 072 };
        }

        [Description(@"
            Options are gfs, ww3 or templated url.
            Url is formatted using .NETs String.Format. 
            Argument 0 is anatime as date, 1 is fctime as timespan and 2 is total fchours as integer
        ")]
        public string Source { get; set; }

        [Description("Runtime hour")]
        public int RunTime { get; set; }

        [Description("Forecast hours")]
        public int[] FcHours { get; set; }
    }
}