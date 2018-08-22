using System.Net;

namespace Sikiro.SMS.SDK.Model
{
    public class Response<T> : Response
    {
        public T Body { get; set; }
    }

    public class Response
    {
        public string Message { get; set; }

        public HttpStatusCode StateCode { get; set; }

        public bool IsSuccess => StateCode == HttpStatusCode.OK;

    }
}
