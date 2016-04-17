using System;

namespace Rabbitual.Infrastructure
{
    public interface IObjectDb
    {
        void Save(object o, string id);
        T Get<T>(string id);
        object Get(Type t, string id);
    }
}