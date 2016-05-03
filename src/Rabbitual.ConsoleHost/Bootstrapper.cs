using System;
using System.Collections.Generic;
using System.Configuration;
using Rabbitual.Configuration;
using Rabbitual.Fox;
using Rabbitual.Infrastructure;
using Rabbitual.Logging;
using Rabbitual.Rabbit;
using StructureMap;
using StructureMap.Graph;
using StructureMap.Pipeline;

namespace Rabbitual.ConsoleHost
{

    public class Bootstrapper
    {
        public static Container Bootstrap(bool inMemory)
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
                    init.For<IMessagePublisher>().Use<RabbitMessagePublisher>();
                    init.For<IEventConsumer>().Use<RabbitEventConsumer>();
                }
                else
                {
                    init.For<Hub>().Use<Hub>().Singleton();
                    init.For<IMessagePublisher>().Use<FoxMessagePublisher>();
                    init.For<IEventConsumer>().Use<FoxEventConsumer>();
                    init.For<ITaskConsumer>().Use<FoxTaskConsumer>();
                }

                init.For<IObjectDb>().Use<FileObjectDb>();
                init.For<IBinarySerializer>().Use<JsonBinarySerializer>();
                init.For<IAgentLogRepository>().Use<AgentLogRepository>().Singleton();
                init.For<IFactory>().Use<Factory>();
                init.For<App>().Use<App>().Singleton();
                init.For<IAgentRepository>().Use(ctx => ctx.GetInstance<App>());
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

            public object GetInstance(Type t, IDictionary<Type, object> deps= null)
            {
                if (deps == null || deps.Count == 0)
                    return _c.GetInstance(t);

                try
                {
                    var args = new ExplicitArguments();
                    foreach (var a in deps)
                        args.Set(a.Key,a.Value);

                    return _c.GetInstance(t, args);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }
        }
    }
}