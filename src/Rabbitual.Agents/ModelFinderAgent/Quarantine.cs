using System;
using System.Collections.Generic;

namespace Rabbitual.Agents.WeatherAgent
{
    public class Quarantine
    {
        IDictionary<string, DateTime> _d = new Dictionary<string, DateTime>();

        public void Add(string url)
        {
            _d[url] = DateTime.UtcNow;
        }

        public bool In(string url)
        {
            var inQ = _d.ContainsKey(url) && _d[url] < DateTime.UtcNow.AddMinutes(5);

            return inQ;
        }

    }
}