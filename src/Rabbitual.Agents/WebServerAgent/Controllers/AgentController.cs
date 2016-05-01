using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Rabbitual.Configuration;

namespace Rabbitual.Agents.WebServerAgent.Controllers
{
    public class AgentCreateController : ApiController
    {

        private readonly IConfigReflection _configReflection;
        private readonly IAgentConfiguration _cfg;

        public AgentCreateController( IConfigReflection configReflection, IAgentConfiguration cfg)
        {
            _configReflection = configReflection;
            _cfg = cfg;
        }

        [HttpGet]
        [Route("agent/types")]
        public HttpResponseMessage Types()
        {
            var agentTypes = _configReflection.GetTypeMap();
            return this.SweetJson(agentTypes.Select(x => x.Key));
        }

        [Route("agent-create")]
        public IHttpActionResult Create([FromBody]CreateCommand body)
        {
            var type = _configReflection.GetTypeMap()[body.Type];

            var agentConfig = new AgentConfigDto
            {
                Id =body.Type + "." + Guid.NewGuid(),
                Name = body.Name,
                Type = body.Type,
                Options = OptionsHelper.CreateDefaultOptionsUsingMagic(type)
            };

            _cfg.PersistConfig(agentConfig);

            return Ok();
        }
    }

    public class CreateCommand
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}