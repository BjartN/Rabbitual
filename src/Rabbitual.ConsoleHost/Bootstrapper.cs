using System;
using Rabbitual.Agents;
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
                    init.For<IMessageConsumer>().Use<RabbitMessageConsumer>();
                }
                else
                {
                    init.For<MessageHub>().Use<MessageHub>().Singleton();
                    init.For<IPublisher>().Use<FoxMessagePublisher>();
                    init.For<IMessageConsumer>().Use<FoxMessageConsumer>();
                }

                init.For<CounterAgent>().Use<CounterAgent>();

                init.For<IAgent[]>().Use(x => new IAgent[]
                {
                    x.GetInstance<CounterAgent>(),
                    x.GetInstance<ScheduledPublisherAgent>(),
                    x.GetInstance<WebCheckerAgent>(),
                });

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