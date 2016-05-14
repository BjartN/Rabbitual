using System;
using System.Linq;
using System.Reflection;
using Rabbitual.Core;

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
        public static object GetStateUsingMagic(IAgent  agent)
        {
            var pi = agent.GetType().GetProperty("State");
            return pi.GetValue(agent, new object[0]);
        }
    }

    public class OptionsHelper
    {
        public static Type GetOptionType(Type agentType)
        {
            return ReflectionHelper.GetGenericArgument(agentType, typeof(IHaveOptions<>));
        }
        
    }
}