using System.Collections.Generic;

namespace Rabbitual.Core.Infrastructure
{
    public class ListManager<T>
    {
        private readonly int _limit;

        public ListManager(List<T> l, int limit)
        {
            _limit = limit;
            List = l;
        }

        public List<T> List { get; set; }

        public void Add(T item)
        {
            if (List.Count >= _limit)
            {
                List.RemoveAt(0);
            }
            List.Add(item);
        }
    }

}