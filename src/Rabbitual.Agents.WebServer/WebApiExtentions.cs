using System.Net.Http;
using System.Text;
using System.Web.Http;
using Rabbitual.Infrastructure;

namespace Rabbitual.Agents.WebServer
{
    public static class WebApiExtentions
    {
        public static HttpResponseMessage SweetJson(this ApiController c, object o, bool bigAssPropertyNames=false, bool keepNulls=false)
        {
            return c.FromRawJson(o.ToJson(bigAssPropertyNames, keepNulls));
        }

        public static HttpResponseMessage FromRawJson(this ApiController c,string rawJson, string contentType= "application/json")
        {
            var response = new HttpResponseMessage
            {
                Content = new StringContent(rawJson, Encoding.UTF8, contentType)
            };
            return response;
        }
    }
}