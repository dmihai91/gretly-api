using System.Runtime.Serialization;

namespace GretlyStudio.Models
{
    public class Token
    {
        [DataMember(Name = "accessToken")]
        public string AccessToken { get; set; }
    }
}
