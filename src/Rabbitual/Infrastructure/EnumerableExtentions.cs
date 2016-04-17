using System.Collections.Generic;

namespace Rabbitual.Infrastructure
{
    public static class EnumerableExtentions
    {
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