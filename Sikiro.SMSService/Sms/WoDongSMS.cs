using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Sikiro.SMSService.Base;

namespace Sikiro.SMSService.Sms
{
    /// <summary>
    /// 杭州沃动短信接口
    /// </summary>
    public class WoDongSMS : BaseSMS
    {
        public WoDongSMS(IConfiguration configuration) : base(configuration)
        {
            var config = configuration.Get<SmsConfig>();
            Account = config.Sms.WoDongSMS.Account;
            Password = config.Sms.WoDongSMS.Password;
            Url = config.Sms.WoDongSMS.Url;
            UserId = config.Sms.WoDongSMS.UserId;
            MaxCount = config.Sms.WoDongSMS.MaxCount;
        }

        public override bool SendSMS(string phone, string content, string signName)
        {
            var utf8Encode = Encoding.GetEncoding("utf-8");

            var param =
                $"action=send&userid={HttpUtility.UrlEncode(UserId, utf8Encode)}&account={HttpUtility.UrlEncode(Account, utf8Encode)}&password={HttpUtility.UrlEncode(Password, utf8Encode)}&mobile={HttpUtility.UrlEncode(phone, utf8Encode)}&content={HttpUtility.UrlEncode(content + signName, utf8Encode)}&json={HttpUtility.UrlEncode("1", utf8Encode)}";

            byte[] postBytes = Encoding.ASCII.GetBytes(param);

            var request = HttpWebRequest.Create(Url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
            request.ContentLength = postBytes.Length;

            using (var reqStream = request.GetRequestStream())
            {
                reqStream.Write(postBytes, 0, postBytes.Length);
            }

            var responseContent = string.Empty;
            using (var response = request.GetResponse())
            {
                //对返回内容进行处理
                using (var sr = new StreamReader(response.GetResponseStream()))
                {
                    responseContent = sr.ReadToEnd();
                }
            }

            if (responseContent != string.Empty)
            {
                var result = JsonConvert.DeserializeObject<ResponseContent>(responseContent);
                if (result.Code == "Success") return true;
            }

            return false;
        }
    }

    class ResponseContent
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("msg")]
        public string Msg { get; set; }

        [JsonProperty("data")]
        public ResponseData Data { get; set; }
    }

    class ResponseData
    {
        [JsonProperty("returnstatus")]
        public string Returnstatus { get; set; }

        [JsonProperty("taskID")]
        public string TaskID { get; set; }

        [JsonProperty("successCounts")]
        public string SuccessCounts { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("remainpoint")]
        public string Remainpoint { get; set; }
    }
}
