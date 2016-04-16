using System.IO;
using Rabbitual.Rabbit;
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

                init.For<IConsumer[]>().Use(x => new IConsumer[]
                {
                    x.GetInstance<TestConsumer>()
                });
            });

            //run service using TopShelf
            HostFactory.Run(x =>
            {
                x.Service<RabbitMessageConsumer>(s =>
                {
                    s.ConstructUsing(name => c.GetInstance<RabbitMessageConsumer>());
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

                x.SetDescription("Processing stuff");
                x.SetDisplayName("Rabbitual Testing 1,2,3");
                x.SetServiceName("Rabbitual Testing 1,2,3");
            });

            var e = c.GetInstance<IPublisher>();
            var counter = 0;

            Timer.DoOnTimer(()=>e.PublishEvent(new TestMessage { Message = "Hello World " + (counter++) }));

            System.Console.ReadLine();
        }
    }
}
