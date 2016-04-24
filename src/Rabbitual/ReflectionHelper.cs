using System;
using System.Linq;
using System.Reflection;

namespace Rabbitual
{
    public class ReflectionHelper
    {

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
        public static object GetPersistedStateUsingMagic(IAgentState service, Type agent)
        {
            var arg = ReflectionHelper.GetGenericArgument(agent, typeof(IStatefulAgent<>));
            var method = typeof(AgentState).GetMethod("GetState");
            var genericMethod = method.MakeGenericMethod(arg);
            return genericMethod.Invoke(service, null);
        }

        public static object GetStateUsingMagic(IAgent  agent)
        {
            var pi = agent.GetType().GetProperty("State");
            return pi.GetValue(agent, new object[0]);
        }


        public static void SetState(IStatefulAgent agent, object result)
        {
            var pi = agent.GetType().GetProperty("State");
            pi.SetValue(agent, result);
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
        public static void SetOptionsUsingMagic(IHaveOptions optionsAgent, object options)
        {
            //TODO: Cheating now, so make more robust
            var pi = optionsAgent.GetType().GetProperties().FirstOrDefault(x => x.Name == "Options");
            if (pi == null)
                return;
            pi.SetValue(optionsAgent, options, null);
        }
    }
}