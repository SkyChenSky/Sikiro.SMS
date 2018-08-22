using System;
using Sikiro.Model;

namespace Sikiro.SMS.Api.Model.Sms
{
    public class SearchModel
    {
        public string Content { get; set; }

        public SmsEnums.SmsType? Type { get; set; }

        public SmsEnums.SmsStatus? Status { get; set; }

        public string Mobile { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? TimeSendDateTime { get; set; }
    }
}
