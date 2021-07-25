using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Gretly.Constants;
using Gretly.Models;

namespace Gretly.Dto
{
    [DataContract]
    public class UserDto
    {
        [DataMember(Name = "id")]
        public readonly string Id;

        [DataMember(Name = "fBaseUserId")]
        public string FBaseUserId { get; set; }

        [DataMember(Name = "fbUserId")]
        public string FBUserId { get; set; }

        [DataMember(Name = "googleUserId")]
        public string GoogleUserId { get; set; }

        [DataMember(Name = "email")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [StringLength(25, ErrorMessage = "{0} length must be maximum {2}")]
        public string Email { get; set; }

        [DataMember(Name = "name")]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 3)]
        [RegularExpression(@"\[^a-zA-Z]", ErrorMessage = "Invalid name")]
        public string Name { get; set; }

        [DataMember(Name = "profilePicture")]
        public string ProfilePicture { get; set; }

        [DataMember(Name = "type")]
        public ProfileType Type { get; set; }

        [DataMember(Name = "phoneNumber")]
        [Phone]
        [StringLength(10, ErrorMessage = "{0} length must be between {1}")]
        public string PhoneNumber { get; set; }

        [DataMember(Name = "country")]
        public string Country { get; set; }

        [DataMember(Name = "createdAt")]
        public DateTime CreatedAt { get; set; }

        [DataMember(Name = "modifiedAt")]
        public DateTime? ModifiedAt { get; set; }

        [DataMember(Name = "lastLoggedIn")]
        public DateTime? LastLoggedIn { get; set; }

        [DataMember(Name = "role")]
        public Role Role { get; set; }

        [DataMember(Name = "verified")]
        public bool Verified { get; set; }

        [DataMember(Name = "locked")]
        public bool Locked { get; set; }

        [DataMember(Name = "companyInfo")]
        public CompanyInfo CompanyInfo { get; set; }

        [DataMember(Name = "education")]
        public Education Education { get; set; }

        [DataMember(Name = "businessAccount")]
        public Boolean BusinessAccount { get; set; }

        [DataMember(Name = "externalLink")]
        public String ExternalLink { get; set; }
    }
}