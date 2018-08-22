using System;
using System.Collections.Specialized;
using Autofac;
using Quartz.Spi;

namespace Quartz.Extension.Autofac
{
    public class QuartzAutofacFactoryModule : Module
    {
        /// <summary>
        ///     Default name for nested lifetime scope.
        /// </summary>
        public const string LifetimeScopeName = "quartz.job";

        readonly string _lifetimeScopeName;

        public QuartzAutofacFactoryModule()
            : this(LifetimeScopeName)
        {
        }

        public QuartzAutofacFactoryModule(string lifetimeScopeName)
        {
            _lifetimeScopeName = lifetimeScopeName ?? throw new ArgumentNullException(nameof(lifetimeScopeName));
        }

        public Func<IComponentContext, NameValueCollection> ConfigurationProvider { get; set; }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(c => new AutofacJobFactory(c.Resolve<ILifetimeScope>(), _lifetimeScopeName))
                .AsSelf()
                .As<IJobFactory>()
                .SingleInstance();

            builder.Register<ISchedulerFactory>(c => {
                var cfgProvider = ConfigurationProvider;

                var autofacSchedulerFactory = cfgProvider != null
                    ? new AutofacSchedulerFactory(cfgProvider(c), c.Resolve<AutofacJobFactory>())
                    : new AutofacSchedulerFactory(c.Resolve<AutofacJobFactory>());
                return autofacSchedulerFactory;
            })
                .SingleInstance();

            builder.Register(c => {
                var factory = c.Resolve<ISchedulerFactory>();
                return factory.GetScheduler().ConfigureAwait(false).GetAwaiter().GetResult();
            })
                .SingleInstance();
        }
    }
}
