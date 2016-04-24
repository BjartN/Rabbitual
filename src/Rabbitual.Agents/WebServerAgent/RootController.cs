using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Rabbitual.Configuration;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WebServerAgent
{
    public class RootController : ApiController
    {
        private readonly IAgentConfiguration _cfg;

        public RootController(IAgentConfiguration cfg)
        {
            _cfg = cfg;
        }

        [Route("agent/{id}")]
        public HttpResponseMessage Get(string id)
        {
            var agent = _cfg.GetConfiguration().FirstOrDefault(x => x.Id == id);

            return this.SweetJson(agent);
        }

        [Route("config")]
        public HttpResponseMessage Get()
        {
            var o = _cfg
                .GetConfiguration()
                .GroupBy(x => x.ClrType)
                .SelectMany(g => g.OrderBy(x => x.Name))
                .Select(x => new
                {
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