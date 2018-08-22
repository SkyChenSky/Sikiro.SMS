using System;
using System.Reflection;
using Autofac;
using Module = Autofac.Module;
namespace Quartz.Extension.Autofac
{
    public class QuartzAutofacJobsModule : Module
    {
        readonly Assembly[] _assembliesToScan;

        public QuartzAutofacJobsModule(params Assembly[] assembliesToScan)
        {
            _assembliesToScan = assembliesToScan ?? throw new ArgumentNullException(nameof(assembliesToScan));
        }

        public bool AutoWireProperties { get; set; }

        public PropertyWiringOptions PropertyWiringOptions { get; set; } = PropertyWiringOptions.None;

        protected override void Load(ContainerBuilder builder)
        {
            var registrationBuilder = builder.RegisterAssemblyTypes(_assembliesToScan)
                .Where(type => !type.IsAbstract && typeof(IJob).IsAssignableFrom(type))
                .AsSelf().InstancePerLifetimeScope();

            if (AutoWireProperties)
                registrationBuilder.PropertiesAutowired(PropertyWiringOptions);
        }
    }
}
