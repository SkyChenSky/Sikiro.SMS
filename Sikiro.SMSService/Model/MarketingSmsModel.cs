using Sikiro.Model;

namespace Sikiro.SMSService.Model
{
    public class MarketingSmsModel
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public SmsEnums.MarketingSmsStatus Status { get; set; }

        public SmsEnums.SmsType SmsMerchant { get; set; }
    }
}
