using System;
using EasyNetQ;
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
                catch (Exception e)
                {
                    _smsService.RollBack();
                    e.WriteToFile();
                }
            });
        }

        public void Stop()
        {
            ConfigServer.Container?.Dispose();
            Console.WriteLine("I stopped");
        }
    }
}
