using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace GretlyStudio.Models
{
    public class LoginInfo
    {
        [Required]
        [DataMember(Name = "username")]
        public string Username { get; set; }

        [Required]
        [DataMember(Name = "password")]
        public string Password { get; set; }
    }
}