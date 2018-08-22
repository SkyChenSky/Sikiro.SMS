using System;
using System.Collections.Concurrent;
using System.Globalization;
using Autofac;
using Quartz.Spi;

namespace Quartz.Extension.Autofac
{
    public class AutofacJobFactory : IJobFactory, IDisposable
    {
        private readonly ILifetimeScope _lifetimeScope;

        private readonly object _scopeTag;

        public AutofacJobFactory(ILifetimeScope lifetimeScope, object scopeTag)
        {
            _lifetimeScope = lifetimeScope ?? throw new ArgumentNullException(nameof(lifetimeScope));
            _scopeTag = scopeTag ?? throw new ArgumentNullException(nameof(scopeTag));
        }

        internal ConcurrentDictionary<object, JobTrackingInfo> RunningJobs { get; } =
            new ConcurrentDictionary<object, JobTrackingInfo>();


        public void Dispose()
        {
            RunningJobs.Clear();
        }

        public virtual IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            if (bundle == null) throw new ArgumentNullException(nameof(bundle));
            if (scheduler == null) throw new ArgumentNullException(nameof(scheduler));

            var jobType = bundle.JobDetail.JobType;

            var nestedScope = _lifetimeScope.BeginLifetimeScope(_scopeTag);

            IJob newJob;
            try
            {
                newJob = (IJob)nestedScope.Resolve(jobType);
                var jobTrackingInfo = new JobTrackingInfo(nestedScope);
                RunningJobs[newJob] = jobTrackingInfo;
                nestedScope = null;
            }
            catch (Exception ex)
            {
                nestedScope?.Dispose();
                throw new SchedulerConfigException(string.Format(CultureInfo.InvariantCulture,
                    "Failed to instantiate Job '{0}' of type '{1}'",
                    bundle.JobDetail.Key, bundle.JobDetail.JobType), ex);
            }
            return newJob;
        }


        public void ReturnJob(IJob job)
        {
            if (job == null)
                return;

            if (!RunningJobs.TryRemove(job, out var trackingInfo))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
                (job as IDisposable)?.Dispose();
            }
            else
            {
                trackingInfo.Scope?.Dispose();
            }
        }

        internal sealed class JobTrackingInfo
        {
            public JobTrackingInfo(ILifetimeScope scope)
            {
                Scope = scope;
            }

            public ILifetimeScope Scope { get; }
        }
    }
}
