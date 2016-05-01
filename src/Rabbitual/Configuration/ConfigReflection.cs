using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbitual.Configuration
{
    public interface IConfigReflection
    {
        IDictionary<string, Type> GetTypeMap();
    }

    public class ConfigReflection : IConfigReflection
    {

        public IDictionary<string, Type> GetTypeMap()
        {
            var type = typeof(IAgent);
            return AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(s => s.GetTypes())
                .Where(a=>!a.IsInterface)
                .Where(a=>!a.IsGenericType)
                .Where(p => type.IsAssignableFrom(p))
                .OrderBy(x=>x.Name)
                .ToDictionary(x => x.Name, x => x);
        }
    }
}