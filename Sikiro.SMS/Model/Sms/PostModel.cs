using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Sikiro.Model;

namespace Sikiro.SMS.Api.Model.Sms
{
    public class PostModel
    {
        [Required, Display(Name = "短信内容")]
        public string Content { get; set; }

        public SmsEnums.SmsType Type { get; set; }

        public List<string> Mobiles { get; set; }

        public DateTime? TimeSendDateTime { get; set; }
    }
}
