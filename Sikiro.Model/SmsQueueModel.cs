using System.Collections.Generic;
using EasyNetQ;

namespace Sikiro.Model
{
    [Queue("Queue.SMS", ExchangeName = "Exchange.SMS")]
    public class SmsQueueModel
    {
        public string _id { get; set; }

        public string Content { get; set; }

        public SmsEnums.SmsType Type { get; set; }

        public List<string> Mobiles { get; set; }
    }
}
