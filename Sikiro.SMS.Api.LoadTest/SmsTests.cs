using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sikiro.SMS.SDK;
using Sikiro.SMS.SDK.Model;

namespace QD.SMS.Api.LoadTest
{
    [TestClass]
    public class SmsTests
    {
        public SmsTests()
        {
            Sms.Init("http://localhost:51511/api");
        }

        [TestMethod]
        public void Sms_Get_IsTrue()
        {
            var result = Sms.Get("9b096de3ad394d1abc81c805145cadaf");
            Assert.AreEqual(result.IsSuccess, true);
        }

        [TestMethod]
        public void Sms_Search_IsTrue()
        {
            var result = Sms.Search(new SearchEntity
            {
                Mobile = "18988561110"
            });

            Assert.AreEqual(result.IsSuccess, true);
        }

        [TestMethod]
        public void Sms_Send_IsTrue()
        {
            var result = Sms.Send(new SendEntity
            {
                Content = "陈珙测试1",
                Mobiles = new List<string>
                {
                    "18988111111",
                    "18988111110",
                    "18988111112",
                    "18988111113"
                },
                Type = SmsType.EXunTong
            });

            Assert.AreEqual(result.IsSuccess, true);
        }

        [TestMethod]
        public void Sms_SendAsync_IsTrue()
        {
            var result = Sms.SendAsync(new SendEntity
            {
                Content = "陈珙测试2",
                Mobiles = new List<string>
                {
                    "18988111111",
                    "18988111110",
                    "18988111112",
                    "18988111113"
                },
                Type = SmsType.EXunTong
            }).Result;

            Assert.AreEqual(result.IsSuccess, true);
        }

        [TestMethod]
        public void Sms_BatchSendAsync_IsTrue()
        {
            var result = Sms.SendAsync(new List<SendEntity>
            {
                new SendEntity
                {
                    Content = "陈珙测试4",
                    Mobiles = new List<string>
                    {
                        "18988111111",
                        "18988111110",
                        "18988111112",
                        "18988111113"
                    },
                    Type = SmsType.EXunTong,
                    TimeSendDateTime =  DateTime.Now.AddMinutes(1)
                },
                new SendEntity
                {
                    Content = "陈珙测试4",
                    Mobiles = new List<string>
                    {
                        "18988111111",
                        "18988111110",
                        "18988111112",
                        "18988111113"
                    },
                    Type = SmsType.EXunTong,
                    TimeSendDateTime =  DateTime.Now.AddMinutes(1)
                },
                new SendEntity
                {
                    Content = "陈珙测试5",
                    Mobiles = new List<string>
                    {
                        "18988111111",
                        "18988111110",
                        "18988111112",
                        "18988111113"
                    },
                    Type = SmsType.EXunTong,
                    TimeSendDateTime = DateTime.Now.AddMinutes(1)
                }
            }).Result;

            Assert.AreEqual(result.IsSuccess, true);
        }
    }
}
