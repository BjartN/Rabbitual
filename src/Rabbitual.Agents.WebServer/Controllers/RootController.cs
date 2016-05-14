using System;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using Rabbitual.Configuration;
using Rabbitual.Logging;

namespace Rabbitual.Agents.WebServer.Controllers
{
    public class RootController : ApiController
    {
        private readonly IAgentConfiguration _cfg;
        private readonly IAgentLogRepository _l;

        public RootController(
            IAgentConfiguration cfg,
            IAgentLogRepository l)
        {
            _cfg = cfg;
            _l = l;
        }

        [HttpGet]
        [Route("agent/message-log/{id}")]
        public HttpResponseMessage MessageLog(int id)
        {
            var al = _l.GetLog(id);

            return this.SweetJson(new
            {
                Incoming = al.GetIncoming(),
                Outgoing = al.GetOutGoing()
            });
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
                    var attr = x.ClrType.GetCustomAttributes(typeof(IconAttribute), true).FirstOrDefault() as IconAttribute;
                    var adminUrl = x.ClrType.GetCustomAttributes(typeof(AdminUrlAttribute), true).FirstOrDefault() as AdminUrlAttribute;

                    return new
                    {
                        AgentType= x.ClrType.Name,
                        Icon = attr==null ?  "hashtag": attr.FontAwesome,
                        AdminUrl = adminUrl?.Url,
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
                        Sources = x.SourceIds,
                        Options = x.Options
                    };
                });

            return this.SweetJson(o);
        }

    }

}