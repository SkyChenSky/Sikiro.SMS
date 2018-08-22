using System;
using PeterKottas.DotNetCore.WindowsService;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Quartz.Spi;

namespace Quartz.Extension
{
    public static class ScheduleJobHostConfiguratorExtensions
    {
        public static HostConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(this HostConfigurator<T> configurator, Func<TJobFactory> jobFactory)
            where TJobFactory : IJobFactory where T : IMicroService
        {
            ScheduleJobServiceConfiguratorExtensions.JobFactory = jobFactory();
            return configurator;
        }

        public static HostConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(this HostConfigurator<T> configurator)
            where TJobFactory : IJobFactory, new() where T : IMicroService
        {
            return UsingQuartzJobFactory(configurator, () => new TJobFactory());
        }
    }
}
