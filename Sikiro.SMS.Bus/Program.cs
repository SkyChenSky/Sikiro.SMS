using System;
using PeterKottas.DotNetCore.WindowsService;

namespace Sikiro.SMS.Bus
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
                    serviceConfig.UseAutofac();
                    serviceConfig.UseServiceFactory();

                    serviceConfig.OnStart((service, extraParams) =>
                    {
                        service.Start();
                    });
                        
                    serviceConfig.OnStop(service =>
                    {
                        service.Stop();
                    });

                    serviceConfig.OnError(Console.WriteLine);
                });
            });
        }
    }
}
