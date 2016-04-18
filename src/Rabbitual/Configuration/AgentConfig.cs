using System;

namespace Rabbitual.Configuration
{
    public class AgentConfig
    {
        public AgentConfig()
        {
            Sources=new AgentConfig[0];
        }


        public string Id { get; set; }
        public string Name { get; set; }
        public int? Schedule { get; set; }
        public Type ClrType { get; set; }

        /// <summary>
        /// If any sources, the agent can only recieve from these sources.
        /// </summary>
        public AgentConfig[] Sources { get; set; }

        public object Options { get; set; }
    }

    public interface IAgentConfiguration
    {
        AgentConfig[] GetConfiguration();
    }

}