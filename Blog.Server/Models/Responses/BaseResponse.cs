using Newtonsoft.Json;

namespace Blog.Server.Models.Responses
{
    public class BaseResponse
    {
        public bool Success { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string? Message { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object? Data { get; set; }

        protected BaseResponse(bool success, string? message = null, object? data = null)
        {
            Success = success;
            Message = message;
            Data = data;
        }

        protected BaseResponse(bool success, string? message = null)
        {
            Success = success;
            Message = message;
        }

        protected BaseResponse(bool success, object? data = null)
        {
            Data = data;
        }

        public static BaseResponse Ok(string? message = null, object? data = null)
        {
            return new BaseResponse(true, message, data);
        }

        public static BaseResponse Fail(string? message = null, object? data = null)
        {
            return new BaseResponse(false, message, data);
        }
    }
}
