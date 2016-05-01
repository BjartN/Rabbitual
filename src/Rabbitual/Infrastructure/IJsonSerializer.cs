using System;

namespace Rabbitual.Infrastructure
{
    public interface IJsonSerializer
    {
        string Serialize<T>(T o);
        T Deserialize<T>(string json);

        object Deserialize(string json, Type t);
    }
}