using Core.ApiResults;
using System.Net;

namespace Infrastructure.Logger
{
    public class UserRequestProperties
    {
        public string Code { get; set; }

        public string TransactionID { get; set; }

        public string TransactionDate { get; set; }

        public string RegisterDate { get; set; }

        public string Params { get; set; }

        public string Response { get; set; }

        public long? Duration { get; set; }

        public string Url { get; set; }

        public string Headers { get; set; }

        public ApiResultStatusCode Status { get; set; }

        public string Error { get; set; }

        public string ClientIP { get; set; }

        public string Host { get; set; }

        public string MiddleLog { get; set; }

        public string Hash { get; set; }

        public string NationalCode { get; set; }

        public string MobileNumber { get; set; }

        public string ServiceType { get; set; }

        public string Method { get; set; }  

        public HttpStatusCode StatusCode { get; set; }
        public string CorrelationId { get; set; }
        public string Description { get; set; }
    }
}
