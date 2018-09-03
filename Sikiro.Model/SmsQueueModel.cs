using System.Collections.Generic;
using EasyNetQ;

namespace Sikiro.Model
{
    [Queue(SmsQueueModelKey.Queue, ExchangeName = SmsQueueModelKey.Exchange)]
    public class SmsQueueModel
    {
        public string _id { get; set; }

        public string Content { get; set; }

        public SmsEnums.SmsType Type { get; set; }

        public List<string> Mobiles { get; set; }
    }

    public static class SmsQueueModelKey
    {
        public const string Queue = "Queue.SMS";
        public const string Exchange = "Exchange.SMS";
        public const string Topic = "Topic.SMS";
    }
}
