using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.PlatformAbstractions;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Configurators.Service;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using QD.Framework;
using QD.Framework.MongoDb;
using QD.Framework.NoSql;
using Quartz;
using Quartz.Extension;
using Quartz.Extension.Autofac;
using Quartz.Spi;

namespace Sikiro.SMS.Job
{
    public static class ConfigServer
    {
        private static readonly IConfiguration Configuration;

        private static readonly Config Config;

        static ConfigServer()
        {
            Configuration = new ConfigurationBuilder()
                .SetBasePath(PlatformServices.Default.Application.ApplicationBasePath)
                .AddJsonFile("Config.json", false, true)
                .Build();

            Config = Configuration.Get<Config>();
        }

        public static void SetServiceInfo<T>(this HostConfigurator<T> config) where T : IMicroService
        {
            config.SetName(Configuration["Server:Name"]);
            config.SetDisplayName(Configuration["Server:DisplayName"]);
            config.SetDescription(Configuration["Server:Description"]);
        }

        public static void InitJob(this ServiceConfigurator<MainService> svc)
        {
            var projectName = Assembly.GetEntryAssembly().FullName.Split(",")[0];
            foreach (var job in Config.Server.Jobs)
            {
                svc.ScheduleQuartzJob(q =>
                {
                    q.WithJob(JobBuilder.Create(Type.GetType($"{projectName}.Jobs.{job.Name}"))
                        .WithIdentity(job.Name, Config.Server.Name)
                        .Build);

                    q.AddTrigger(() => TriggerBuilder.Create()
                        .WithCronSchedule(job.Cron)
                        .Build());
                });
            }
        }

        public static void UseAutofac(this ServiceConfigurator<MainService> svc)
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule(new QuartzAutofacFactoryModule());
            builder.RegisterModule(new QuartzAutofacJobsModule(typeof(MainService).Assembly));

            builder.Register(r => Configuration).As<IConfiguration>().SingleInstance();

            builder.Register(a => new MongoProxy(Configuration["Infrastructure:Mongodb"])).As<IMongoProxy>().SingleInstance();

            builder.RegisterAssemblyTypes(Assembly.Load("Sikiro.SMSService"))
                .Where(t => t.GetInterfaces().Any(a => a == typeof(IService)))
                .InstancePerLifetimeScope();

            builder.RegisterEasyNetQ(Configuration["Infrastructure:RabbitMQ"]);

            var container = builder.Build();

            svc.UsingQuartzJobFactory(container.Resolve<IJobFactory>);
        }
    }
}
