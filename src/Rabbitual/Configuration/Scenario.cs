namespace Rabbitual.Configuration
{
    public class ScenarioConfig
    {

        public ScenarioConfig(AgentConfig[] configs)
        {
            Scenario = configs;
        }

        public AgentConfig[] Scenario { get; set; }
    }
}