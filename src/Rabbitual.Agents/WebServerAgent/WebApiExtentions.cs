using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WebServerAgent
{
    public static class WebApiExtentions
    {
        public static HttpResponseMessage SweetJson(this ApiController c, object o)
        {
            var response = new HttpResponseMessage();
            response.Content = new StringContent(o.ToJson());
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            return response;
        }
    }
}