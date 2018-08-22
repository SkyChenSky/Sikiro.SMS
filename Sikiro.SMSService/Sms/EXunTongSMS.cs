using System;
using System.IO;
using System.Net;
using System.Text;
using Microsoft.Extensions.Configuration;
using Sikiro.SMSService.Base;

namespace Sikiro.SMSService.Sms
{
    /// <summary>
    ///E讯通短信接口
    /// </summary>
    public class EXunTongSMS : BaseSMS
    {

        public EXunTongSMS(IConfiguration configuration) : base(configuration)
        {
            var config = configuration.Get<SmsConfig>();
            Account = config.Sms.EXunTongSMS.Account;
            Password = config.Sms.EXunTongSMS.Password;
            Url = config.Sms.EXunTongSMS.Url;
            UserId = config.Sms.EXunTongSMS.UserId;
            MaxCount = config.Sms.EXunTongSMS.MaxCount;
        }

        public override bool SendSMS(string phone, string content, string signName)
        {
            try
            {
                var seqId = Guid.NewGuid().ToString();
                var pwd_MD5 = MD5Encrypt(Password);
                var sign = MD5Encrypt(seqId + pwd_MD5);
                var id = new Random().Next(1, int.MaxValue);
                var json = "{'id':" + id + ",'method':'send','params':{'userid':'" + UserId + "','seqid':'" + seqId + "','sign':'" + sign + "','submit':[{'phone':'" + phone + "','content':'" + content + "'}]}}";
                var apiUrl = Url + "?" + json;

                var req = (HttpWebRequest)WebRequest.Create(apiUrl);
                req.Method = "GET";
                var res = req.GetResponse();
                var responseReader = new StreamReader(res.GetResponseStream());
                var str = responseReader.ReadToEnd();

                if (str.IndexOf("error") < 0 && str.IndexOf("id") >= 0 && str.IndexOf("成功") >= 0)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static string MD5Encrypt(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}
