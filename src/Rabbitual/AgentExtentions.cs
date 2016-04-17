using System.Runtime.Remoting.Messaging;

namespace Rabbitual
{
    public static class AgentExtentions
    {
        public static int? TryGetIntOption(this IAgent a, string key)
        {
            var agent = a as IOptionsAgent;
            if (agent == null)
                return null;

            string value;
            if (agent.Options.TryGetValue(key, out value))
            {
                int finalValue;
                if (int.TryParse(value, out finalValue))
                {
                    return finalValue;
                }
            }

            return null;
        }
    }
}