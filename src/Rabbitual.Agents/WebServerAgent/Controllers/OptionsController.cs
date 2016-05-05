using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NJsonSchema;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;
using Rabbitual.Logging;

namespace Rabbitual.Agents.WebServerAgent.Controllers
{
    public class OptionsController:ApiController
    {
        private readonly IAgentConfiguration _cfg;
        private readonly IAgentConfiguration _configRepository;
        private readonly IJsonSerializer _serializer;

        public OptionsController(
            IAgentConfiguration cfg,
            IAgentConfiguration configRepository,
            IJsonSerializer serializer)
        {
            _cfg = cfg;
            _configRepository = configRepository;
            _serializer = serializer;
        }

        [HttpGet]
        [Route("agent/options/schema/{id}")]
        public HttpResponseMessage Schema(string id)
        {
            var cfg = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);
            if (cfg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var schema = JsonSchema4.FromType(cfg.Options.GetType());

            return this.FromRawJson(schema.ToJson(), "application/schema+json");
        }


        [Route("agent/options/update/{id}")]
        public HttpResponseMessage Update(string id)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var cfg = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);
            if (cfg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var newOptions = _serializer.Deserialize(json, cfg.Options.GetType());
            cfg.Options = newOptions;
            _configRepository.PersistConfig(cfg.ToDto());

            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        [HttpGet]
        [Route("agent/options/{id}")]
        public HttpResponseMessage Options(string id)
        {
            var cfg = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);
            if (cfg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return this.SweetJson(cfg.Options);
        }

        /// <summary>
        ///     Use fat options to support schema-form generation
        /// </summary>
        [HttpGet]
        [Route("agent/fat-options/{id}")]
        public HttpResponseMessage FatOptions(string id)
        {
            var cfg = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);
            if (cfg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return this.SweetJson(cfg.Options, bigAssPropertyNames: true, keepNulls: true);
        }

    }
}