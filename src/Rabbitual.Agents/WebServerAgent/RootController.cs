using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using NJsonSchema;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WebServerAgent
{
    public class RootController : ApiController
    {
        private readonly IAgentConfiguration _cfg;
        private readonly IAgentRepository _ar;
        private readonly IAgentService _s;
        private readonly IAgentLogRepository _l;
        private readonly IAgentConfiguration _configRepository;
        private readonly IJsonSerializer _serializer;

        public RootController(
            IAgentConfiguration cfg,
            IAgentRepository ar,
            IAgentService s,
            IAgentLogRepository l,
            IAgentConfiguration configRepository,
            IJsonSerializer serializer)
        {
            _cfg = cfg;
            _ar = ar;
            _s = s;
            _l = l;
            _configRepository = configRepository;
            _serializer = serializer;
        }

        [HttpGet]
        [Route("agent/options/schema/{id}")]
        public HttpResponseMessage Schema(string id)
        {
            var agent = _ar.GetAgent(id);
            var schema = JsonSchema4.FromType(agent.Config.Options.GetType());

            var response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(schema.ToJson(), Encoding.UTF8, "application/schema+json");
            return response;
        }


        [Route("agent/options/update/{id}")]
        public IHttpActionResult Update(string id)
        {
            var json = Request.Content.ReadAsStringAsync().Result;
            var agent = _ar.GetAgent(id);
            if (agent == null)
                return NotFound();

            var newOptions = _serializer.Deserialize(json, agent.Config.Options.GetType());
            //Note: This will replace the options object, so the agent(s) will still keep a refrence to the old one.
            //Could be solved my replacing just copying the properties one by one.
            agent.Config.Options = newOptions;

            _configRepository.PersistConfig(agent.Config.ToDto());


            return Ok();
        }


        [HttpGet]
        [Route("agent/options/{id}")]
        public HttpResponseMessage Options(string id)
        {
            var agent = _ar.GetAgent(id);
            return this.SweetJson(agent.Config.Options, bigAssPropertyNames: true);
        }

        [HttpGet]
        [Route("agent/message-log/{id}")]
        public HttpResponseMessage MessageLog(string id)
        {
            var al = _l.GetLog(id);

            return this.SweetJson(new
            {
                Incoming = al.GetIncoming(),
                Outgoing = al.GetOutGoing()
            });
        }


        [Route("agent/state/{id}")]
        public HttpResponseMessage Get(string id)
        {
            var agent = _ar.GetAgent(id);
            var state = _s.GetState(agent.Agent);

            return this.SweetJson(state);
        }

        [Route("config")]
        public HttpResponseMessage Get()
        {
            var root = Request.RequestUri.GetLeftPart(UriPartial.Authority);

            var o = _cfg
                .GetConfiguration()
                .GroupBy(x => x.ClrType)
                .SelectMany(g => g.OrderBy(x => x.Name))
                .Select(x =>
                {
                    var al = _l.GetLog(x.Id).GetSummary();

                    return new
                    {
                        OutgoingCount = al.OutgoingCount,
                        IncomingCount = al.IncomingCount,
                        LastCheck = al.LastCheck == null ? null : new DateTime?(al.LastCheck.Occured),
                        LastEventIn = al.LastEventIn == null ? null : new DateTime?(al.LastEventIn.Occured),
                        LastEventOut = al.LastEventOut == null ? null : new DateTime?(al.LastEventOut.Occured),
                        LastTaskIn = al.LastTaskIn == null ? null : new DateTime?(al.LastTaskIn.Occured),
                        LastTaskOut = al.LastTaskOut == null ? null : new DateTime?(al.LastTaskOut.Occured),
                        MessageLogUrl = $"{root}/agent/message-log/{x.Id}",
                        StateUrl = $"{root}/agent/state/{x.Id}",
                        OptionsSchemaUrl = $"{root}/agent/options/schema?id={x.Id}",
                        x.Id,
                        x.Name,
                        Sources = x.Sources.Select(s => s.Id),
                        Options = x.Options
                    };
                });

            return this.SweetJson(o);
        }

    }

}