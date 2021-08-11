using System.Runtime.Serialization;

namespace GretlyStudio.Dto
{
    public class Error
    {
        public string Code { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }

    [DataContract]
    public class ErrorResponse
    {
        [DataMember(Name = "error")]
        public Error Error { get; set; }
    }
}