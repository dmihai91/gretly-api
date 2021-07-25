using System.ComponentModel.DataAnnotations;

namespace Gretly.Models
{
    public class AuthResponse
    {
        [Display(Name = "accessToken")]
        public string AccessToken { get; set; }

        [Display(Name = "refreshToken")]
        public string RefreshToken { get; set; }

        [Display(Name = "expiresIn")]
        public int ExpiresIn { get; set; }

        [Display(Name = "sessionId")]
        public string SessionId { get; set; }
    }
}