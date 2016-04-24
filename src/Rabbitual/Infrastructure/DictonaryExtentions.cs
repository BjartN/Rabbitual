﻿using System;
using System.Collections.Generic;

namespace Rabbitual.Infrastructure
{
    public static class DictonaryExtentions
    {
        public static double? AsDouble(this IDictionary<string, string> d, string key)
        {
            string v;
            if (!d.TryGetValue(key, out v))
                return null;

            double dbl;
            if (!double.TryParse(v, out dbl))
                return null;

            return dbl;
        }
    }
}