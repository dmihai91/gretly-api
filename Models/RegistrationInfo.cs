using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace GretlyStudio.Models
{
    public class RegistrationInfo
    {
        [DataMember(Name = "username")]
        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}", MinimumLength = 3)]
        [RegularExpression(@"^[a-z0-9-_]+$", ErrorMessage = "Invalid username (only lower case alphanumeric underscore and dash characters are allowed)")]
        public string Username { get; set; }

        [DataMember(Name = "email")]
        [Required]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(25, ErrorMessage = "{0} length must be maximum {2}")]
        public string Email { get; set; }

        [DataMember(Name = "password")]
        [Required]
        [StringLength(25, ErrorMessage = "{0} length must be between {2} and {1}", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$", ErrorMessage = "Weak password it should contains a minimum of 8 characters and it should contains lowercase, uppercase and special characters")]
        public string Password { get; set; }

        [DataMember(Name = "confirmPassword")]
        [Required]
        [StringLength(25, ErrorMessage = "{0} length must be between {2} and {1}", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        [RegularExpression(@"^.*(?=.{6,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[!*@#$%^&+=]).*$", ErrorMessage = "Weak password it should contains a minimum of 6 characters and it should contains lowercase, uppercase and special characters")]
        public string ConfirmPassword { get; set; }

        [DataMember(Name = "name")]
        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
        [RegularExpression(@"^(?:((([^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]'’,\-.\s])){1,}(['’,\-\.]){0,1}){2,}(([^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]'’,\-. ]))*(([ ]+){0,1}(((([^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]'’,\-\.\s])){1,})(['’\-,\.]){0,1}){2,}((([^0-9_!¡?÷?¿/\\+=@#$%ˆ&*(){}|~<>;:[\]'’,\-\.\s])){2,})?)*)$", ErrorMessage = "Invalid name (only unicode characters and space are allowed)")]
        public string Name { get; set; }
    }
}