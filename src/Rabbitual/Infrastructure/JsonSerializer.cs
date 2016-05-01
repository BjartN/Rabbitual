using System;
using Newtonsoft.Json;

namespace Rabbitual.Infrastructure
{
    public class JsonSerializer : IJsonSerializer
    {
        public string Serialize<T>(T o)
        {
            return o.ToJson();
        }

        public T Deserialize<T>(string json)
        {
            return  JsonConvert.DeserializeObject<T>(json);
        }

        public object Deserialize(string json, Type t)
        {
            return JsonConvert.DeserializeObject(json,t);
        }
    }
}