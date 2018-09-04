using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Sikiro.Model;
using Sikiro.Nosql.Mongo;
using Sikiro.SMS.Toolkits;
using Sikiro.SMSService.Interfaces;
using Sikiro.SMSService.Model;
using Sikiro.SMSService.Sms;

namespace Sikiro.SMSService
{
    public class SmsService : IService
    {
        private readonly SmsFactory _smsFactory;
        private readonly IConfiguration _configuration;
        private readonly MongoRepository _mongoProxy;

        public List<SmsModel> SmsList { get; set; }

        public SmsModel Sms { get; set; }

        public SmsService(SmsFactory smsFactory, IConfiguration configuration, MongoRepository mongoProxy)
        {
            _smsFactory = smsFactory;
            _configuration = configuration;
            _mongoProxy = mongoProxy;
        }

        public SmsService GetToBeSend()
        {
            Sms = _mongoProxy.GetAndUpdate<SmsModel>(
                a => a.Status == SmsEnums.SmsStatus.待处理 && a.TimeSendDateTime <= DateTime.Now,
                a => new SmsModel { Status = SmsEnums.SmsStatus.处理中 });
            return this;
        }

        public SmsService Get(string id)
        {
            Sms = _mongoProxy.Get<SmsModel>(a => a.Id == id);
            return this;
        }

        public SmsService Send()
        {
            Send(Sms);
            return this;
        }

        public void Send(SmsModel item)
        {
            Sms = item;

            var isSuccess = _smsFactory.Create(item.Type).SendSMS(item.Mobiles, item.Content, _configuration["Sms:SignName"]);
            if (isSuccess)
                Success(item.Id);
            else
                Fail(item.Id);
        }

        public void ContinueDo(Action todo)
        {
            if (Sms != null)
                todo();
        }

        public void Add(List<AddSmsModel> smsModels)
        {
            DateTime now = DateTime.Now;

            var smsModel = new List<SmsModel>();
            foreach (var sms in smsModels)
            {
                var maxCount = _smsFactory.Create(sms.Type).MaxCount;
                sms.Mobiles = sms.Mobiles.Distinct().ToList();
                var page = GetPageCount(sms.Mobiles.Count, maxCount);

                var index = 0;
                do
                {
                    var toBeSendPhones = sms.Mobiles.Skip(index * maxCount).Take(maxCount).ToList();
                    smsModel.Add(new SmsModel
                    {
                        Content = sms.Content,
                        CreateDateTime = now,
                        Mobiles = toBeSendPhones,
                        TimeSendDateTime = sms.TimeSendDateTime,
                        Type = sms.Type
                    });
                    index++;
                } while (index < page);
            }

            SmsList = smsModel;

            _mongoProxy.BatchAdd(SmsList);
        }

        public void Search(SearchSmsModel searchSmsModel)
        {
            var builder = ExpressionBuilder.Init<SmsModel>();
            if (searchSmsModel != null)
            {
                if (searchSmsModel.Status.HasValue)
                    builder = builder.And(a => a.Status == searchSmsModel.Status.Value);

                if (searchSmsModel.Type.HasValue)
                    builder = builder.And(a => a.Type == searchSmsModel.Type.Value);

                if (searchSmsModel.BeganCreateDateTime.HasValue)
                    builder = builder.And(a => a.CreateDateTime >= searchSmsModel.BeganCreateDateTime.Value);

                if (searchSmsModel.EndCreateDateTime.HasValue)
                    builder = builder.And(a => a.CreateDateTime <= searchSmsModel.EndCreateDateTime.Value);

                if (searchSmsModel.BeganTimeSendDateTime.HasValue)
                    builder = builder.And(a => a.TimeSendDateTime >= searchSmsModel.BeganTimeSendDateTime.Value);

                if (searchSmsModel.EndTimeSendDateTime.HasValue)
                    builder = builder.And(a => a.TimeSendDateTime <= searchSmsModel.EndTimeSendDateTime.Value);

                if (!string.IsNullOrEmpty(searchSmsModel.Mobile))
                    builder = builder.And(a => a.Mobiles.Contains(searchSmsModel.Mobile));

                if (!string.IsNullOrEmpty(searchSmsModel.Content))
                    builder = builder.And(a => a.Content.Contains(searchSmsModel.Content));
            }

            SmsList = _mongoProxy.ToList(builder);
        }

        public void RollBack()
        {
            RollBack(Sms.Id);
        }

        public void RollBack(string id)
        {
            _mongoProxy.Update<SmsModel>(a => a.Id == id,
                a => new SmsModel { Status = SmsEnums.SmsStatus.待处理 });
        }

        private void Success(string id)
        {
            _mongoProxy.Update<SmsModel>(a => a.Id == id,
                a => new SmsModel { Status = SmsEnums.SmsStatus.成功 });
        }

        private void Fail(string id)
        {
            _mongoProxy.Update<SmsModel>(a => a.Id == id,
                a => new SmsModel { Status = SmsEnums.SmsStatus.失败 });
        }

        private int GetPageCount(int phoneCount, int maxCount)
        {
            return (int)Math.Ceiling(phoneCount / (double)maxCount);
        }
    }
}
