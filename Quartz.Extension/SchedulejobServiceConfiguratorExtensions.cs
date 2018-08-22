using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using log4net.Repository;
using PeterKottas.DotNetCore.WindowsService.Configurators.Service;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Quartz.Impl;
using Quartz.Spi;

namespace Quartz.Extension
{
    public static class ScheduleJobServiceConfiguratorExtensions
    {
        private static readonly ILoggerRepository Repository = LogManager.CreateRepository("QuartzRepository");
        private static readonly ILog Log = LogManager.GetLogger(Repository.Name, typeof(ScheduleJobServiceConfiguratorExtensions));

        private static readonly Func<IScheduler> DefaultSchedulerFactory = () =>
        {
            var schedulerFactory = new StdSchedulerFactory();
            return schedulerFactory.GetScheduler().Result;
        };

        private static Func<IScheduler> _customSchedulerFactory;
        private static IScheduler _scheduler;
        internal static IJobFactory JobFactory;

        public static Func<IScheduler> SchedulerFactory
        {
            get => _customSchedulerFactory ?? DefaultSchedulerFactory;
            set => _customSchedulerFactory = value;
        }

        private static IScheduler GetScheduler()
        {
            var scheduler = SchedulerFactory();

            if (JobFactory != null)
                scheduler.JobFactory = JobFactory;

            return scheduler;
        }

        public static ServiceConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(
            this ServiceConfigurator<T> configurator, Func<TJobFactory> jobFactory)
            where T : class, IMicroService
            where TJobFactory : IJobFactory
        {
            JobFactory = jobFactory();
            return configurator;
        }

        public static ServiceConfigurator<T> UsingQuartzJobFactory<T, TJobFactory>(this ServiceConfigurator<T> configurator)
            where T : class, IMicroService where TJobFactory : IJobFactory, new()
        {
            return UsingQuartzJobFactory(configurator, () => new TJobFactory());
        }

        public static ServiceConfigurator<T> ScheduleQuartzJob<T>(this ServiceConfigurator<T> configurator,
            Action<QuartzConfigurator> jobConfigurator, bool replaceJob = false) where T : class, IMicroService
        {
            ConfigureJob(configurator, jobConfigurator, replaceJob);
            return configurator;
        }
        public delegate void InitJob(IScheduler scheduler);

        public static event InitJob InitJobEvent;

        private static void ConfigureJob<T>(ServiceConfigurator<T> configurator,
            Action<QuartzConfigurator> jobConfigurator, bool replaceJob = false) where T : class, IMicroService
        {
            var jobConfig = new QuartzConfigurator();
            jobConfigurator(jobConfig);

            if (jobConfig.JobEnabled == null || jobConfig.JobEnabled() ||
                (jobConfig.Job == null || jobConfig.Triggers == null))
            {
                var jobDetail = jobConfig.Job();
                var jobTriggers = jobConfig.Triggers.Select(triggerFactory => triggerFactory())
                    .Where(trigger => trigger != null);
                var jobListeners = jobConfig.JobListeners;
                var triggerListeners = jobConfig.TriggerListeners;
                var scheduleListeners = jobConfig.ScheduleListeners;

                InitJobEvent += (scheduler) =>
                {
                    if (scheduler != null && jobDetail != null && jobTriggers.Any())
                    {
                        var triggersForJob = new HashSet<ITrigger>(jobTriggers);
                        scheduler.ScheduleJob(jobDetail, triggersForJob, replaceJob);
                        Log.Info(string.Format("[Topshelf.Quartz] Scheduled Job: {0}", jobDetail.Key));

                        foreach (var trigger in triggersForJob)
                            Log.Info(string.Format("[Topshelf.Quartz] Job Schedule: {0} - Next Fire Time (local): {1}",
                                trigger,
                                trigger.GetNextFireTimeUtc().HasValue
                                    ? trigger.GetNextFireTimeUtc().Value.ToLocalTime().ToString()
                                    : "none"));

                        if (jobListeners.Any())
                        {
                            foreach (var listener in jobListeners)
                            {
                                var config = listener();
                                scheduler.ListenerManager.AddJobListener(
                                    config.Listener, (IMatcher<JobKey>[])config.Matchers);
                                Log.Info(
                                    string.Format(
                                        "[Topshelf.Quartz] Added Job Listener: {0}",
                                        config.Listener.Name));
                            }
                        }

                        if (triggerListeners.Any())
                        {
                            foreach (var listener in triggerListeners)
                            {
                                var config = listener();
                                scheduler.ListenerManager.AddTriggerListener(config.Listener,
                                    (IMatcher<TriggerKey>[])config.Matchers);
                                Log.Info(
                                    string.Format(
                                        "[Topshelf.Quartz] Added Trigger Listener: {0}",
                                        config.Listener.Name));
                            }
                        }
                        if (scheduleListeners.Any())
                        {
                            foreach (var listener in scheduleListeners)
                            {
                                var schedListener = listener();
                                scheduler.ListenerManager.AddSchedulerListener(schedListener);
                                string.Format(
                                    "[Topshelf.Quartz] Added Schedule Listener: {0}",
                                    schedListener.GetType());
                            }

                        }

                    }
                };
                configurator.OnStart((a, b) =>
                {
                    Log.Debug("[Topshelf.Quartz] Scheduler starting up...");
                    if (_scheduler == null)
                        _scheduler = GetScheduler();

                    InitJobEvent?.Invoke(_scheduler);

                    _scheduler.Start();
                    Log.Info("[Topshelf.Quartz] Scheduler started...");
                });

                configurator.OnStop(a =>
                {
                    Log.Debug("[Topshelf.Quartz] Scheduler shutting down...");
                    if (_scheduler != null)
                        if (!_scheduler.IsShutdown)
                            _scheduler.Shutdown();
                    Log.Info("[Topshelf.Quartz] Scheduler shut down...");
                });

            }
        }
    }
}
