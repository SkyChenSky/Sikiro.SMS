using System.Collections.Generic;

namespace Quartz.Extension
{
    public class QuartzJobListenerConfig
    {
        public IJobListener Listener { get; set; }
        public IList<IMatcher<JobKey>> Matchers { get; set; }

        public QuartzJobListenerConfig(IJobListener listener, params IMatcher<JobKey>[] matchers)
        {
            Listener = listener;
            Matchers = matchers;
        }

    }
}