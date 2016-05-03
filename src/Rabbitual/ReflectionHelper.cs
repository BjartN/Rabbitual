using System;
using System.Linq;
using System.Reflection;

namespace Rabbitual
{
    public static class ReflectionHelper
    {
        public static bool IsOfType(this Type o, Type target)
        {
            if (target.IsGenericType && target.IsInterface)
            {
                return o.GetInterfaces().Any(x => x.IsGenericType &&   x.GetGenericTypeDefinition() == target);
            }

            //sick naming
            return target.IsAssignableFrom(o);
        }

        public static Type[] GetArgumentsOfOpenGenericInterfaceType(Type openGenericType, Assembly[] assemblies)
        {
            return GetArgumentsOfOpenGenericInterfaceType(openGenericType,
                assemblies.SelectMany(a => a.GetTypes()).ToArray());
        }

        public static Type[] GetArgumentsOfOpenGenericInterfaceType(Type openGenericType, Type[] types)
        {
            return types
                .Select(t=> GetGenericArgument(t, openGenericType))
                .Where(t=>t!=null)
                .ToArray();
        }

        public static Type GetGenericArgument(Type type, Type interfaceType)
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

    }

    public class StateHelper
    {
        public static Type GetStateType(Type agent)
        {
            return ReflectionHelper.GetGenericArgument(agent, typeof(IStatefulAgent<>));
        }

        public static object GetPersistedStateUsingMagic(IAgentStateRepository service, Type agent)
        {
            var stateType = GetStateType(agent);
            var method = typeof(AgentStateRepository).GetMethod("GetState");
            var genericMethod = method.MakeGenericMethod(stateType);
            return genericMethod.Invoke(service, null);
        }

        public static object GetStateUsingMagic(IAgent  agent)
        {
            var pi = agent.GetType().GetProperty("State");
            return pi.GetValue(agent, new object[0]);
        }
    }

    public class OptionsHelper
    {

        public static object[] GetDefaultOptions(Assembly[] assemblies)
        {
            var optionTypes = ReflectionHelper.GetArgumentsOfOpenGenericInterfaceType(typeof(IHaveOptions<>), assemblies);
            return optionTypes.Select(Activator.CreateInstance).ToArray();
        }

        public static object CreateDefaultOptionsUsingMagic(Type agentType)
        {
            var optionType = GetOptionType(agentType);
            return Activator.CreateInstance(optionType);
        }

        public static Type GetOptionType(Type agentType)
        {
            return ReflectionHelper.GetGenericArgument(agentType, typeof(IHaveOptions<>));
        }
        
    }
}