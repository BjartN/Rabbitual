using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WebServerAgent.Controllers
{
    public class AgentCreateController : ApiController
    {
        private readonly IJsonSerializer _serializer;
        private readonly IConfigReflection _configReflection;
        private readonly IAgentConfiguration _cfg;
        private readonly IAgentPool _ar;
        private readonly IAgentService _s;

        public AgentCreateController(IJsonSerializer serializer, IConfigReflection configReflection, IAgentConfiguration cfg,
             IAgentPool ar,
            IAgentService s)
        {
            _serializer = serializer;
            _configReflection = configReflection;
            _cfg = cfg;
            _ar = ar;
            _s = s;
        }

        [HttpGet]
        [Route("agent/types")]
        public HttpResponseMessage Types()
        {
            var agentTypes = _configReflection.GetTypeMap();
            return this.SweetJson(agentTypes.Select(x => x.Key));
        }

        [Route("agent/config/update/{id}")]
        public HttpResponseMessage Update(string id)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var cfg = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);
            if (cfg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var newOptions = _serializer.Deserialize<AgentConfigDto>(json);
            cfg.Name = newOptions.Name;
            cfg.SourceIds = newOptions.SourceIds.Clean();
            _cfg.PersistConfig(cfg.ToDto());

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [Route("agent-create")]
        public IHttpActionResult Create([FromBody]CreateCommand body)
        {
            var type = _configReflection.GetTypeMap()[body.Type];

            var agentConfig = new AgentConfigDto
            {
                Id = body.Type + "." + Guid.NewGuid(),
                Name = body.Name,
                Type = body.Type,
                Options = OptionsHelper.CreateDefaultOptionsUsingMagic(type)
            };

            _cfg.PersistConfig(agentConfig);

            return Ok();
        }


        [Route("agent/state/{id}")]
        public HttpResponseMessage Get(string id)
        {
            var agent = _ar.GetAgent(id);
            var state = _s.GetState(agent);

            return this.SweetJson(state);
        }

    }

    public class CreateCommand
    {
        public string Name { get; set; }
        public string Type { get; set; }
    }
}