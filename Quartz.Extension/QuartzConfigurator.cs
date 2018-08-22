using System;
using System.Collections.Generic;

namespace Quartz.Extension
{
    public class QuartzConfigurator
    {
        public Func<IJobDetail> Job { get; set; }
        public IList<Func<ITrigger>> Triggers { get; set; }
        public IList<Func<QuartzJobListenerConfig>> JobListeners { get; set; }
        public IList<Func<QuartzTriggerListenerConfig>> TriggerListeners { get; set; }
        public IList<Func<ISchedulerListener>> ScheduleListeners { get; set; }
        public Func<bool> JobEnabled { get; set; }

        public QuartzConfigurator()
        {
            Triggers = new List<Func<ITrigger>>();
            TriggerListeners = new List<Func<QuartzTriggerListenerConfig>>();
            JobListeners = new List<Func<QuartzJobListenerConfig>>();
            ScheduleListeners = new List<Func<ISchedulerListener>>();
        }

        public QuartzConfigurator WithJob(Func<IJobDetail> jobDetail)
        {
            Job = jobDetail;
            return this;
        }

        public QuartzConfigurator AddTrigger(Func<ITrigger> jobTrigger)
        {
            Triggers.Add(jobTrigger);
            return this;
        }

        public QuartzConfigurator WithTriggers(IEnumerable<ITrigger> jobTriggers)
        {
            foreach (var jobTrigger in jobTriggers)
            {
                var trigger = jobTrigger;
                AddTrigger(() => trigger);
            }
            return this;
        }

        public QuartzConfigurator EnableJobWhen(Func<bool> jobEnabled)
        {
            JobEnabled = jobEnabled;
            return this;
        }

        public QuartzConfigurator WithJobListener(Func<QuartzJobListenerConfig> jobListener)
        {
            JobListeners.Add(jobListener);
            return this;
        }

        public QuartzConfigurator WithTriggerListener(Func<QuartzTriggerListenerConfig> triggerListener)
        {
            TriggerListeners.Add(triggerListener);
            return this;
        }

        public QuartzConfigurator WithScheduleListener(Func<ISchedulerListener> scheduleListener)
        {
            ScheduleListeners.Add(scheduleListener);
            return this;
        }
    }
}