using System;
using System.Threading.Tasks;
using Quartz;
using Sikiro.SMS.Toolkits;

namespace Sikiro.SMS.Job.Jobs
{
    public abstract class BaseJob : IJob
    {
        private void OnException(Action action)
        {
            try
            {
                action();
            }
            catch (Exception e)
            {
                e.WriteToFile();
                OnException();
            }
        }

        public Task Execute(IJobExecutionContext context)
        {
            OnException(ExecuteBusiness);

            return null;
        }

        protected virtual void OnException()
        {

        }

        protected abstract void ExecuteBusiness();
    }
}
