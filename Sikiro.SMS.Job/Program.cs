using System;
using PeterKottas.DotNetCore.WindowsService;

namespace Sikiro.SMS.Job
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceRunner<MainService>.Run(config =>
            {
                config.SetServiceInfo();

                config.Service(serviceConfig =>
                {
                    serviceConfig.ServiceFactory((extraArguments, controller) => new MainService());

                    serviceConfig.OnStart((service, extraParams) =>
                    {
                        service.Start();
                    });

                    serviceConfig.OnStop(service =>
                    {
                        service.Stop();
                    });

                    serviceConfig.OnError(Console.WriteLine);

                    serviceConfig.UseAutofac();

                    serviceConfig.InitJob();
                });
            });
        }
    }
}
