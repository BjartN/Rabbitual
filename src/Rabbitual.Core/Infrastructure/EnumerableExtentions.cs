﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbitual.Core.Infrastructure
{
    public static class EnumerableExtentions
    {
        public static bool ContainsKeys<T>(this IDictionary<string,T> d, params string[] keys)
        {
            return keys.All(d.ContainsKey);
        }

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        /// <summary>
        /// Adds object to the list and returns it. Nice for chaining.
        /// </summary>
        public static T Push<T>(this List<T> l, T o)
        {
            l.Add(o);
            return o;
        }

    }
}