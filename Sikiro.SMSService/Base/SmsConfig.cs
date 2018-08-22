namespace Sikiro.SMSService.Base
{
    public class SmsConfig
    {
        public Sms Sms { get; set; }
    }

    public class Sms
    {
        public string SignName { get; set; }

        public Jianzhousms JianZhouSMS { get; set; }

        public Wodongsms WoDongSMS { get; set; }

        public EXuntongsms EXunTongSMS { get; set; }
    }

    public class Jianzhousms
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public int MaxCount { get; set; }
    }

    public class Wodongsms
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public int MaxCount { get; set; }
    }

    public class EXuntongsms
    {
        public string Account { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string UserId { get; set; }
        public int MaxCount { get; set; }
    }
}
