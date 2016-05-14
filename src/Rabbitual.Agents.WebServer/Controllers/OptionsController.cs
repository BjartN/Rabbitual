using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NJsonSchema;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WebServer.Controllers
{
    public class OptionsController:ApiController
    {
        private readonly IAgentDb _db;
        private readonly IAgentConfiguration _cfg;
        private readonly IJsonSerializer _serializer;

        public OptionsController(
            IAgentDb db,
            IAgentConfiguration cfg,
            IJsonSerializer serializer)
        {
            _db = db;
            _cfg = cfg;
            _serializer = serializer;
        }

        [HttpGet]
        [Route("agent/options/schema/{id}")]
        public HttpResponseMessage Schema(int id)
        {
            var cfg = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);
            if (cfg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            //TODO: Will crash for agents without options

            var schema = JsonSchema4.FromType(cfg.Options.GetType());

            return this.FromRawJson(schema.ToJson(), "application/schema+json");
        }


        [Route("agent/options/update/{id}")]
        public HttpResponseMessage Update(int id)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var cfg = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);
            if (cfg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var newOptions = _serializer.Deserialize(json, cfg.Options.GetType());
            _db.InsertOrReplaceOptions(id,newOptions);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        [HttpGet]
        [Route("agent/options/{id}")]
        public HttpResponseMessage Options(int id)
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
        public HttpResponseMessage FatOptions(int id)
        {
            var cfg = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);
            if (cfg == null)
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            return this.SweetJson(cfg.Options, bigAssPropertyNames: true, keepNulls: true);
        }

    }
}