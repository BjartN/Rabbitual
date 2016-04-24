using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using Rabbitual.Agents;
using Rabbitual.Agents.WebCheckerAgent;
using Rabbitual.Configuration;
using Rabbitual.Fox;
using Rabbitual.Infrastructure;
using Rabbitual.Rabbit;
using StructureMap;
using StructureMap.Graph;

namespace Rabbitual.ConsoleHost
{

    public class Bootstrapper
    {
        public static Container Bootstrap(bool inMemory, IAgentConfiguration configuraton)
        {
            var c = new Container(init =>
            {
                init.Scan(scanner =>
                {
                    //look in all dll's
                    scanner.AssembliesFromApplicationBaseDirectory();

                    //and make all interfaces of type IFoo be implemented by class of type Foo
                    scanner.WithDefaultConventions();
                });

                if (!inMemory)
                {
                    init.For<IPublisher>().Use<RabbitMessagePublisher>();
                    init.For<IEventConsumer>().Use<RabbitEventConsumer>();
                }
                else
                {
                    init.For<Hub>().Use<Hub>().Singleton();
                    init.For<IPublisher>().Use<FoxMessagePublisher>();
                    init.For<IEventConsumer>().Use<FoxEventConsumer>();
                    init.For<ITaskConsumer>().Use<FoxTaskConsumer>();

                }

                init.For<IObjectDb>().Use<FileObjectDb>();
                init.For<ISerializer>().Use<JsonSerializer>();
                init.For<IAgentConfiguration>().Use(configuraton);
                init.For<IFactory>().Use<Factory>();

                init.For<App>().Use<App>().Singleton();
            });

            return c;
        }

        public class Factory : IFactory
        {
            private readonly IContainer _c;

            public Factory(IContainer c)
            {
                _c = c;
            }

            public object GetInstance(Type t)
            {
                return _c.GetInstance(t);
            }
        }
    }
}