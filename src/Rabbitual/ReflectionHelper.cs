using System;
using System.Linq;
using System.Reflection;

namespace Rabbitual
{
    public class ReflectionHelper
    {
        public static object[] GetDefaultOptions(Assembly[] assemblies)
        {
            var optionTypes = GetArgumentsOfOpenGenericInterfaceType(typeof(IHaveOptions<>), assemblies);
            return optionTypes.Select(Activator.CreateInstance).ToArray();
        }

        public static object CreateDefaultOptionsUsingMagic(Type agentType)
        {
            var optionType = GetOptionType(agentType);
            return Activator.CreateInstance(optionType);
        }

        public static Type GetOptionType(Type agentType)
        {
            return GetGenericArgument(agentType, typeof(IHaveOptions<>));
        }

        private static Type[] GetArgumentsOfOpenGenericInterfaceType(Type openGenericType, Assembly[] assemblies)
        {
            return GetArgumentsOfOpenGenericInterfaceType(openGenericType,
                assemblies.SelectMany(a => a.GetTypes()).ToArray());
        }

        private static Type[] GetArgumentsOfOpenGenericInterfaceType(Type openGenericType, Type[] types)
        {
            return types
                .Select(t=> GetGenericArgument(t, openGenericType))
                .Where(t=>t!=null)
                .ToArray();
        }

        private static Type GetGenericArgument(Type type, Type interfaceType)
        {
            if (type.IsInterface || type.IsAbstract)
                return null;

            foreach (var i in type.GetInterfaces()) { 
                if (i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType) { 
                    return i.GetGenericArguments()[0];
                }
            }

            return null;
        }

        public static void SetOptionsUsingMagic(IHaveOptions optionsAgent, object options)
        {
            //TODO: Cheating now, so make more robust
            var pi = optionsAgent.GetType().GetProperties().FirstOrDefault(x => x.Name == "Options");
            if (pi == null)
                return;
            pi.SetValue(optionsAgent,options, null);
        }
    }
}