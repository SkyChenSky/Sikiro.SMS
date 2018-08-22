using System;
using Sikiro.Model;

namespace Sikiro.SMSService.Model
{
    public class SearchSmsModel
    {
        public string Content { get; set; }

        public SmsEnums.SmsType? Type { get; set; }

        public SmsEnums.SmsStatus? Status { get; set; }

        public string Mobile { get; set; }

        public DateTime? BeganCreateDateTime { get; set; }

        public DateTime? EndCreateDateTime { get; set; }

        public DateTime? BeganTimeSendDateTime { get; set; }

        public DateTime? EndTimeSendDateTime { get; set; }
    }
}
