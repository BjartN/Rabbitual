using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbitual.Infrastructure
{
    public static class EnumerableExtentions
    {
        private static Random rng = new Random();

        public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
        {
            return source.Skip(Math.Max(0, source.Count() - N));
        }

        public static IList<T> Shuffle<T>(this IList<T> a)
        {
            var list = a.ToList();

            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        /// <summary>
        /// Adds object to the list and returns it. Nice for chaining.
        /// </summary>
        public static T Push<T>(this List<T> l, T o)
        {
            l.Add(o);
            return o;
        }

        public static T[] ToAry<T>(this T o)
        {
            return new T[] {o};
        }
    }
}