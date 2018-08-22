using System;
using System.Collections.Specialized;
using Quartz.Core;
using Quartz.Impl;

namespace Quartz.Extension.Autofac
{
    public class AutofacSchedulerFactory : StdSchedulerFactory
    {
        private readonly AutofacJobFactory _jobFactory;

        public AutofacSchedulerFactory(AutofacJobFactory jobFactory)
        {
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
        }

        public AutofacSchedulerFactory(NameValueCollection props, AutofacJobFactory jobFactory)
            : base(props)
        {
            _jobFactory = jobFactory ?? throw new ArgumentNullException(nameof(jobFactory));
        }

        protected override IScheduler Instantiate(QuartzSchedulerResources rsrcs, QuartzScheduler qs)
        {
            var scheduler = base.Instantiate(rsrcs, qs);
            scheduler.JobFactory = _jobFactory;
            return scheduler;
        }
    }
}
