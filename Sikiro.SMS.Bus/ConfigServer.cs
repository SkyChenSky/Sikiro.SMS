using System.Linq;
using System.Reflection;
using Autofac;
using EasyNetQ.Scheduling;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Configurators.Service;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Sikiro.Nosql.Mongo;
using Sikiro.SMSService.Interfaces;

namespace Sikiro.SMS.Bus
{
    public static class ConfigServer
    {
        private static readonly IConfiguration Configuration;

        public static IContainer Container;

        static ConfigServer()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                .AddJsonFile("Config.json", false, true)
                .Build();
        }

        public static void SetServiceInfo<T>(this HostConfigurator<T> config) where T : IMicroService
        {
            config.SetName(Configuration["Server:Name"]);
            config.SetDisplayName(Configuration["Server:DisplayName"]);
            config.SetDescription(Configuration["Server:Description"]);
        }

        public static void UseAutofac(this ServiceConfigurator<MainService> svc)
        {
            var builder = new ContainerBuilder();

            builder.Register(r => Configuration).As<IConfiguration>().SingleInstance();

            builder.Register(a => new MongoRepository(Configuration["Infrastructure:Mongodb"])).SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.Load("Sikiro.SMSService"))
                .Where(t => t.GetInterfaces().Any(a => a == typeof(IService)))
                .InstancePerLifetimeScope();

            builder.RegisterType<MainService>().SingleInstance();
            builder.RegisterEasyNetQ(Configuration["Infrastructure:RabbitMQ"], config =>
            {
                config.EnableDeadLetterExchangeAndMessageTtlScheduler();
            });

            Container = builder.Build();
        }

        public static void UseServiceFactory(this ServiceConfigurator<MainService> svc)
        {
            svc.ServiceFactory((extraArguments, controller) => Container.Resolve<MainService>());
        }
    }
}
