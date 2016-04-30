using System.Web.Http;
using System.Web.Http.Dispatcher;
using Owin;

namespace Rabbitual.Agents.WebServerAgent
{
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            config.MessageHandlers.Add(new CustomHeaderHandler());
            config.MapHttpAttributeRoutes();
            config.Services.Replace(typeof(IHttpControllerActivator), new ServiceActivator(config, Factory));
            appBuilder.UseWebApi(config);
        }

        public static IFactory Factory { get; set; }
    }
}