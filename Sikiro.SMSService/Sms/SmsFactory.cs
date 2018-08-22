using Sikiro.Model;
using Sikiro.SMSService.Base;
using Sikiro.SMSService.Interfaces;

namespace Sikiro.SMSService.Sms
{
    public class SmsFactory : IService
    {
        private readonly WoDongSMS _woDongSms;
        private readonly JianZhouSMS _jianZhouSms;
        private readonly EXunTongSMS _eXunTongSms;
        public SmsFactory(WoDongSMS woDongSms, JianZhouSMS jianZhouSms, EXunTongSMS eXunTongSms)
        {
            _woDongSms = woDongSms;
            _jianZhouSms = jianZhouSms;
            _eXunTongSms = eXunTongSms;
        }

        public BaseSMS Create(SmsEnums.SmsType type)
        {
            switch (type)
            {
                case SmsEnums.SmsType.JisnZhou: return _jianZhouSms;
                case SmsEnums.SmsType.WoDong: return _woDongSms;
                case SmsEnums.SmsType.EXunTong: return _eXunTongSms;
                default: throw new SmsException("无法识别的type");
            }
        }
    }
}
