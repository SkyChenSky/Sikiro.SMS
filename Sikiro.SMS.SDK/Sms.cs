using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using RestSharp;
using RestSharp.Serializers;
using Sikiro.SMS.SDK.Model;

namespace Sikiro.SMS.SDK
{
    /// <summary>
    /// 短信服务SDK
    /// </summary>
    public static class Sms
    {
        private static RestClient _client;

        private static string Host { get; set; }

        public static void Init(string host)
        {
            Host = host;
            _client = new RestClient(Host);
        }

        /// <summary>
        /// 批量发送短信（异步）
        /// </summary>
        /// <param name="sendList"></param>
        /// <returns></returns>
        public static async Task<Response> SendAsync(List<SendEntity> sendList)
        {
            var request = new RestRequest("sms", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(sendList);

            var response = await _client.ExecuteTaskAsync(request);

            return ToResponse(response);
        }

        /// <summary>
        /// 单条发送短信（异步）
        /// </summary>
        /// <param name="sendEntity"></param>
        /// <returns></returns>
        public static async Task<Response> SendAsync(SendEntity sendEntity)
        {
            return await SendAsync(new List<SendEntity> { sendEntity });
        }

        /// <summary>
        /// 批量发送短信（同步）
        /// </summary>
        /// <param name="sendList"></param>
        /// <returns></returns>
        public static Response Send(List<SendEntity> sendList)
        {
            var request = new RestRequest("sms", Method.POST) { RequestFormat = DataFormat.Json };

            request.AddBody(sendList);

            var response = _client.Execute(request);

            return ToResponse(response);
        }

        /// <summary>
        /// 单条发送短信（异步）
        /// </summary>
        /// <param name="sendEntity"></param>
        /// <returns></returns>
        public static Response Send(SendEntity sendEntity)
        {
            return Send(new List<SendEntity> { sendEntity });
        }

        /// <summary>
        /// 获取单个SMS记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Response<SearchResponse> Get(string id)
        {
            var request = new RestRequest("sms/{id}", Method.GET);

            request.AddUrlSegment("id", id);

            var response = _client.Execute<SearchResponse>(request);

            return ToResponse(response);
        }

        /// <summary>
        /// 查询SMS记录
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public static Response<List<SearchResponse>> Search(SearchEntity searchModel)
        {
            var request = new RestRequest("sms/_search", Method.POST) { RequestFormat = DataFormat.Json };
            request.AddBody(searchModel);

            var response = _client.Execute<List<SearchResponse>>(request);

            return ToResponse(response);
        }

        private static Response<T> ToResponse<T>(IRestResponse<T> t)
        {
            var msg = t.IsSuccessful ? t.StatusCode.ToString() : t.Content;
            return new Response<T> { Body = t.Data, StateCode = t.StatusCode, Message = t.ErrorMessage ?? msg, IsSuccess = t.IsSuccessful };
        }

        private static Response ToResponse(IRestResponse t)
        {
            var msg = t.IsSuccessful ? t.StatusCode.ToString() : t.Content;
            return new Response { StateCode = t.StatusCode, Message = t.ErrorMessage ?? msg, IsSuccess = t.IsSuccessful };
        }
    }

    public class SendEntity
    {
        public string Content { get; set; }

        public SmsType Type { get; set; }

        private List<string> _mobiles;

        public List<string> Mobiles
        {
            get => _mobiles ?? new List<string>();
            set => _mobiles = value;
        }

        public DateTime? TimeSendDateTime { get; set; }
    }

    public class SearchEntity
    {
        public string Content { get; set; }

        public SmsType? Type { get; set; }

        public SmsStatus? Status { get; set; }

        public string Mobile { get; set; }

        public DateTime? CreateDateTime { get; set; }

        public DateTime? TimeSendDateTime { get; set; }
    }

    public class SearchResponse
    {
        public SearchResponse()
        {
            Mobiles = new List<string>();
        }

        public string Content { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public List<string> Mobiles { get; set; }

        [SerializeAs(Name = "_id", Attribute = true)]
        public string Id { get; set; }
    }
}
