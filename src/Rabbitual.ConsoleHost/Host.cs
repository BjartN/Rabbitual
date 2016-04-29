using Rabbitual.Configuration;
using Topshelf;

namespace Rabbitual.ConsoleHost
{
    public static class Host
    {
        public static void Run(bool inMemory)
        {
            var c = Bootstrapper.Bootstrap(true);

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
