using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Hosting;
using Rabbitual.Core.Logging;

namespace Rabbitual.Agents.WebServer
{
    public class WebServer
    {
        private readonly ILogger _l;
        private IDisposable _server;

        public WebServer(ILogger l, IFactory f)
        {
            _l = l;

            //TODO: Figure out how to build properly
            Startup.Factory = f;
        }

        public void Start()
        {
            var baseAddress = "http://localhost:9000/";
            _l.Debug($"Creating server at {baseAddress}");
            _server = WebApp.Start<Startup>(url: baseAddress);

        }

        public void Stop()
        {
            _server.Dispose();
        }
    }

    public class ServiceActivator : IHttpControllerActivator
    {
        private readonly IFactory _f;
        public ServiceActivator(HttpConfiguration configuration, IFactory f)
        {
            _f = f;
        }

        public IHttpController Create(HttpRequestMessage request
            , HttpControllerDescriptor controllerDescriptor, Type controllerType)
        {
            var controller = _f.GetInstance(controllerType) as IHttpController;
            return controller;
        }

    }
}