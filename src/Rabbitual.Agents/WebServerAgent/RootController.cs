using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WebServerAgent
{
    public class RootController : ApiController
    {
        private readonly IAgentConfiguration _cfg;
        private readonly IAgentRepository _ar;
        private readonly IAgentService _s;

        public RootController(
            IAgentConfiguration cfg, 
            IAgentRepository ar,
            IAgentService s)
        {
            _cfg = cfg;
            _ar = ar;
            _s = s;
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
                .Select(x => new
                {
                    Url = $"{root}/agent/state/{x.Id}",
                    x.Id,
                    x.Name,
                    NumSources = x.Sources.Length,
                    Options =x.Options?.GetType().GetProperties().Where(p => p.CanRead).Select(p => new
                    {
                        p.Name,
                        Value = p.GetValue(x.Options)
                    })
                });

            return this.SweetJson(o);
        }

    }

    
}