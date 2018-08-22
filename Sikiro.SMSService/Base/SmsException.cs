using System;

namespace Sikiro.SMSService.Base
{
    public class SmsException : ApplicationException
    {
        public SmsException(string msg) : base(msg)
        {
        }
    }
}
