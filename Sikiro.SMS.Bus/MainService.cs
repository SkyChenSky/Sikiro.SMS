using System;
using EasyNetQ;
using EasyNetQ.Scheduling;
using PeterKottas.DotNetCore.WindowsService.Interfaces;
using Sikiro.Model;
using Sikiro.SMS.Toolkits;
using Sikiro.SMSService;

namespace Sikiro.SMS.Bus
{
    public class MainService : IMicroService
    {
        private readonly IBus _bus;
        private readonly SmsService _smsService;

        public MainService(IBus bus, SmsService smsService)
        {
            _bus = bus;
            _smsService = smsService;
        }

        public void Start()
        {
            Console.WriteLine("I started");

            _bus.Subscribe<SmsQueueModel>("", msg =>
            {
                try
                {
                    _smsService.Send(msg.MapTo<SmsQueueModel, SmsModel>());
                }
                catch (TimeoutException e)
                {
                    e.WriteToFile();

                    ReSend();
                }
                catch (Exception e)
                {
                    e.WriteToFile();
                }
            }, a =>
            {
                a.WithTopic(SmsQueueModelKey.Topic);
            });
        }

        private void ReSend()
        {
            var model = _smsService.Sms.MapTo<SmsModel, SmsQueueModel>();
            model.SendCount++;

            _bus.FuturePublish(TimeSpan.FromSeconds(30 * model.SendCount), model, SmsQueueModelKey.Topic);
        }

        public void Stop()
        {
            ConfigServer.Container?.Dispose();
            Console.WriteLine("I stopped");
        }
    }
}
