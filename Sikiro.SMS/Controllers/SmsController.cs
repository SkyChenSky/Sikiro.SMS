using System;
using System.Collections.Generic;
using System.Linq;
using EasyNetQ;
using EasyNetQ.Scheduling;
using Microsoft.AspNetCore.Mvc;
using Sikiro.Model;
using Sikiro.SMS.Api.Model.Sms;
using Sikiro.SMS.Toolkits;
using Sikiro.SMSService;
using Sikiro.SMSService.Model;

namespace Sikiro.SMS.Api.Controllers
{
    /// <summary>
    /// 短信接口
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SmsController : ControllerBase
    {
        private readonly SmsService _smsService;
        private readonly IBus _bus;

        public SmsController(SmsService smsService, IBus bus)
        {
            _smsService = smsService;
            _bus = bus;
        }

        /// <summary>
        /// 获取短信记录
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult<SmsModel> Get(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var smsService = _smsService.Get(id);
            return smsService.Sms;
        }

        /// <summary>
        /// 添加短信记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post([FromBody] List<PostModel> model)
        {
            var allSmsList = _smsService.Page(model.MapTo<List<PostModel>, List<AddSmsModel>>()).SmsList;

            allSmsList.Where(a => a.TimeSendDateTime == null).ToList().MapTo<List<SmsModel>, List<SmsQueueModel>>()
                .ForEach(
                    item =>
                    {
                        _bus.Publish(item, SmsQueueModelKey.Topic);
                    });

            allSmsList.Where(a => a.TimeSendDateTime != null).ToList()
                .ForEach(
                    item =>
                    {
                        _bus.FuturePublish(item.TimeSendDateTime.Value.ToUniversalTime(), item.MapTo<SmsModel, SmsQueueModel>(),
                            SmsQueueModelKey.Topic);
                    });

            return Ok();
        }

        /// <summary>
        /// 查询短信记录
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("_search")]
        public ActionResult<List<SmsModel>> Post([FromBody] SearchModel model)
        {
            _smsService.Search(model.MapTo<SearchModel, SearchSmsModel>());

            return _smsService.SmsList;
        }
    }
}
