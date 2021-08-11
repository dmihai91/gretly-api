using GretlyStudio.Dto;
using Newtonsoft.Json;

namespace GretlyStudio.Utils
{
    public class ApiError
    {

        public string Reason { get; set; }

        public string Message { get; set; }

        public ErrorResponse Errors { get; set; }

        public ApiError(string reason, string message, string responseData)
        {
            this.Reason = reason;
            this.Message = message;
            this.Errors = JsonConvert.DeserializeObject<ErrorResponse>(responseData);
        }

        public ApiError(string reason, string message)
        {
            this.Reason = reason;
            this.Message = message;
        }

        public object getErrorWithoutResponseData()
        {
            return new
            {
                reason = this.Reason,
                message = this.Message
            };
        }
    }
}