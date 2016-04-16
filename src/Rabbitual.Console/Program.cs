using System.IO;
using Rabbitual.Agents;
using Rabbitual.Infrastructure;
using Rabbitual.Rabbit;
using Rabbitual.Rabbit.Fake;
using StructureMap;
using StructureMap.Graph;
using Topshelf;

namespace Rabbitual.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            //setup ioc container
            var c = new Container(init =>
            {
                init.Scan(scanner =>
                {
                    //look in all dll's
                    scanner.AssembliesFromApplicationBaseDirectory();

                    //and make all interfaces of type IFoo be implemented by class of type Foo
                    scanner.WithDefaultConventions();
                });

                init.For<IPublisher>().Use<RabbitMessagePublisher>();
                init.For<ISerializer>().Use<JsonSerializer>();
                init.For<IMessageConsumer>().Use<TimedFakeMessageConsumer>();
                init.For<App>().Use(x =>new App(x.GetInstance<IMessageConsumer>(),new IAgent[]
                {
                    x.GetInstance<CounterAgent>(),
                    x.GetInstance<TestAgent>()
                }));
            });

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
    }
}
