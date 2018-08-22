using System;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;
using Sikiro.SMSService.Base;

namespace Sikiro.SMSService.Sms
{
    /// <summary>
    /// 上海建周短信接口
    /// </summary>
    public class JianZhouSMS : BaseSMS
    {
        public JianZhouSMS(IConfiguration configuration) : base(configuration)
        {
            var config = Configuration.Get<SmsConfig>();
            Account = config.Sms.JianZhouSMS.Account;
            Password = config.Sms.JianZhouSMS.Password;
            Url = config.Sms.JianZhouSMS.Url;
            MaxCount = config.Sms.JianZhouSMS.MaxCount;
        }

        public override bool SendSMS(string phone, string content, string signName)
        {
            Encoding myEncoding = Encoding.GetEncoding("utf-8");
            string param = HttpUtility.UrlEncode("account", myEncoding) +
            "=" + HttpUtility.UrlEncode(Account, myEncoding) +
            "&" + HttpUtility.UrlEncode("password", myEncoding) +
            "=" + HttpUtility.UrlEncode(Password, myEncoding) +
            "&" + HttpUtility.UrlEncode("destmobile", myEncoding) +
            "=" + HttpUtility.UrlEncode(phone, myEncoding) +
            "&" + HttpUtility.UrlEncode("msgText", myEncoding) +
            "=" + HttpUtility.UrlEncode(string.Format("{0}{1}", content, signName), myEncoding) +
            "&" + HttpUtility.UrlEncode("sendDateTime", myEncoding) +
            "=" + HttpUtility.UrlEncode("", myEncoding);

            byte[] postBytes = Encoding.ASCII.GetBytes(param);

            var req = HttpWebRequest.Create(Url);
            req.Method = "POST";
            req.ContentType =
            "application/x-www-form-urlencoded;charset=utf-8";
            req.ContentLength = postBytes.Length;

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(postBytes, 0, postBytes.Length);

            }
            string responseContent;
            using (WebResponse wr = req.GetResponse())
            {
                //对返回内容进行处理
                using (var sr = new StreamReader(wr.GetResponseStream()))
                {
                    responseContent = sr.ReadToEnd();
                }

            }
            return responseContent != string.Empty && Convert.ToInt32(responseContent) > 0;

        }
    }
}
