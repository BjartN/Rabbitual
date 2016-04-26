using System;
using System.Collections.Generic;
using System.Globalization;

namespace Rabbitual.Infrastructure
{
    public static class DictonaryExtentions
    {
        public static double? TryGetDouble(this IDictionary<string, string> d, string key)
        {
            string v;
            if (!d.TryGetValue(key, out v))
                return null;

            double dbl;
            if (!double.TryParse(v, NumberStyles.Any, CultureInfo.InvariantCulture, out dbl))
                return null;

            return dbl;
        }
    }
}