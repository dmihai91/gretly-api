using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace GretlyStudio.Models
{

    public class FBPictureData
    {
        [DataMember(Name = "data")]
        public FBPicture Data { get; set; }
    }

    public class FBData
    {
        [DataMember(Name = "accessToken")]
        public string AccessToken { get; set; }

        [DataMember(Name = "id")]
        [Required]
        public string Id { get; set; }

        [DataMember(Name = "email")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(25, ErrorMessage = "{0} length must be maximum {2}")]
        public string Email { get; set; }

        [DataMember(Name = "name")]
        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 5)]
        [RegularExpression(@"^(?:((([^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]'’,\-.\s])){1,}(['’,\-\.]){0,1}){2,}(([^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]'’,\-. ]))*(([ ]+){0,1}(((([^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]'’,\-\.\s])){1,})(['’\-,\.]){0,1}){2,}((([^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]'’,\-\.\s])){2,})?)*)$", ErrorMessage = "Invalid name (only unicode characters and space are allowed)")]
        public string Name { get; set; }

        [DataMember(Name = "picture")]
        [Required]
        public FBPictureData Picture { get; set; }
    }
}