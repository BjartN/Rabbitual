using System;
using System.Collections.Generic;

namespace Rabbitual
{
    /// <summary>
    /// Used for creating objects dynamically after the application has started.
    /// </summary>
    public interface IFactory
    {
        object GetInstance(Type t, IDictionary<Type, object> deps = null);
    }
}