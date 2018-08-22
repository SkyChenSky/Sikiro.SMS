using EasyNetQ;
using Quartz;
using Sikiro.Model;
using Sikiro.SMS.Toolkits;
using Sikiro.SMSService;

namespace Sikiro.SMS.Job.Jobs
{
    /// <summary>
    /// 发送定时信息
    /// </summary>
    [DisallowConcurrentExecution]
    public class TimeSendSms : BaseJob
    {
        private readonly SmsService _smsService;
        private readonly IBus _bus;

        public TimeSendSms(SmsService smsService, IBus bus)
        {
            _smsService = smsService;
            _bus = bus;
        }

        protected override void ExecuteBusiness()
        {
            _smsService.GetToBeSend();

            if (_smsService.Sms != null)
                _bus.Publish(_smsService.Sms.MapTo<SmsModel, SmsQueueModel>());

            _smsService.ContinueDo(ExecuteBusiness);
        }

        protected override void OnException()
        {
            _smsService.RollBack();
        }
    }
}
