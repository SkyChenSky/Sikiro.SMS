using System;
using System.Collections.Generic;
using Sikiro.Model;

namespace Sikiro.SMSService.Model
{
    public class AddSmsModel
    {
        public string Content { get; set; }

        public SmsEnums.SmsType Type { get; set; }

        public List<string> Mobiles { get; set; }

        public DateTime? TimeSendDateTime { get; set; }
    }
}
