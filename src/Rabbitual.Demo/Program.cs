using System.IO;
using Rabbitual.Agents;
using Rabbitual.Demo.ThirdPartyAgent;
using Rabbitual.Fox;
using Rabbitual.Infrastructure;
using Rabbitual.Rabbit;
using StructureMap;
using StructureMap.Graph;
using Topshelf;

namespace Rabbitual.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var foo = ReflectionHelper.GetDefaultOptions(new[] {typeof(ScheduledPublisherAgent).Assembly});

            var animal = args.Length > 0 ? args[0] : "fox";
            var inMemory = animal == "fox";

            //setup ioc container
            var c = bootstrap(inMemory);

            //run service using TopShelf
            HostFactory.Run(x =>
            {
                x.Service<App>(s =>
                {
                    s.ConstructUsing(name => c.GetInstance<App>());
                    s.WhenStarted(tc => tc.Start());
                    s.WhenStopped(tc => tc.Stop());
                });

                x.EnableServiceRecovery(r =>
                {
                    r.RestartService(0);

                    //number of days until the error count resets
                    r.SetResetPeriod(1);
                });

                x.StartAutomatically(); //...when installed
                x.RunAsLocalSystem();

                x.SetDescription("Rabbitual Demo");
                x.SetDisplayName("Rabbitual Demo");
                x.SetServiceName("Rabbitual.Demo");
            });

            System.Console.ReadLine();
        }

        private static Container bootstrap(bool inMemory)
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
                init.For<ScheduledPublisherAgent>().Use<ScheduledPublisherAgent>();

                init.For<IAgent[]>().Use(x => new IAgent[]
                {
                    x.GetInstance<CounterAgent>(),
                    x.GetInstance<ScheduledPublisherAgent>()
                });

                init.For<IObjectDb>().Use<FileObjectDb>();
                init.For<ISerializer>().Use<JsonSerializer>();

                init.For<App>().Use<App>(ctx => new App(
                    ctx.GetInstance<IMessageConsumer>(),
                    ctx.GetInstance<IAgent[]>(),
                    ctx.GetInstance<IObjectDb>(),
                    ctx.GetInstance<ILogger>(),
                    ctx.GetInstance<IOptionsRepository>())).Singleton();
            });

            return c;
        }
    }
}
