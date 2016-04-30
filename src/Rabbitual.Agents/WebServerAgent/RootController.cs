using System;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;
using Rabbitual.Configuration;

namespace Rabbitual.Agents.WebServerAgent
{
    public class RootController : ApiController
    {
        private readonly IAgentConfiguration _cfg;
        private readonly IAgentRepository _ar;
        private readonly IAgentService _s;
        private readonly IAgentLogRepository _l;

        public RootController(
            IAgentConfiguration cfg, 
            IAgentRepository ar,
            IAgentService s,
            IAgentLogRepository l)
        {
            _cfg = cfg;
            _ar = ar;
            _s = s;
            _l = l;
        }

        [Route("agent/update")]
        public bool Update()
        {
            return true;
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
                        x.Id,
                        x.Name,
                        Sources = x.Sources.Select(s => s.Id),
                        Options = x.Options?.GetType().GetProperties().Where(p => p.CanRead).Select(p => new
                        {
                            Description = getDescription(p),
                            p.Name,
                            Value = p.GetValue(x.Options)
                        })
                    };
                });

            return this.SweetJson(o);
        }

        private string getDescription(PropertyInfo pi)
        {
            var attr = pi.GetCustomAttribute<DescriptionAttribute>();
            if (attr == null)
                return null;

            return attr.Description;

        }
    }


}