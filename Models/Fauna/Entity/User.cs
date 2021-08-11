using System;
using FaunaDB.Types;
using GretlyStudio.Constants;
using GretlyStudio.Dto;
using GretlyStudio.Utils;
using Newtonsoft.Json;

namespace GretlyStudio.Models
{
    public class User : FaunaEntity
    {
        [FaunaField("fBaseUserId")]
        public string FBaseUserId { get; set; } = null;

        [FaunaField("fbUserId")]
        public string FBUserId { get; set; } = null;

        [FaunaField("googleUserId")]
        public string GoogleUserId { get; set; } = null;

        [FaunaField("username")]
        public string Username { get; set; }

        [FaunaField("email")]
        public string Email { get; set; }

        [FaunaField("name")]
        public string Name { get; set; }

        [FaunaField("profilePicture")]
        public string ProfilePicture { get; set; } = null;

        [FaunaField("phoneNumber")]
        public string PhoneNumber { get; set; } = null;

        [FaunaField("country")]
        public string Country { get; set; } = null;

        [FaunaField("createdAt")]
        public DateTime CreatedAt { get; set; }

        [FaunaField("modifiedAt")]
        public DateTime? ModifiedAt { get; set; } = null;

        [FaunaField("lastLoggedIn")]
        public DateTime? LastLoggedIn { get; set; } = null;

        [FaunaField("roleRef")]
        [JsonIgnore]
        public RefV RoleRef;

        [FaunaIgnore]
        public Role Role { get; set; }

        [FaunaField("verified")]
        public bool Verified { get; set; } = false;

        [FaunaField("locked")]
        public bool Locked { get; set; } = false;

        [FaunaField("companyInfo")]
        public CompanyInfo CompanyInfo { get; set; } = null;

        [FaunaField("education")]
        public Education Education { get; set; } = null;

        [FaunaField("businessAccount")]
        public bool BusinessAccount { get; set; } = false;

        [FaunaField("externalLink")]
        public string ExternalLink { get; set; } = "";

        [FaunaField("isEnrolled")]
        public bool IsEnrolled { get; set; } = false;

        [FaunaConstructor]
        public User(string fBaseUserId, string fBUserId, string googleUserId, string email, string name, string profilePicture, string phoneNumber, string country, DateTime createdAt, RefV roleRef, bool verified, bool locked, CompanyInfo companyInfo, Education education, bool businessAccount, string externalLink, bool isEnrolled)
        {
            FBaseUserId = fBaseUserId;
            FBUserId = fBUserId;
            GoogleUserId = googleUserId;
            Email = email;
            Name = name;
            ProfilePicture = profilePicture;
            PhoneNumber = phoneNumber;
            Country = country;
            CreatedAt = createdAt;
            RoleRef = roleRef;
            Verified = verified;
            Locked = locked;
            CompanyInfo = companyInfo;
            Education = education;
            BusinessAccount = businessAccount;
            ExternalLink = externalLink;
            IsEnrolled = isEnrolled;
            LinkRefs();
        }

        // used by json deserializer
        [JsonConstructor]
        public User(string fBaseUserId, string fBUserId, string googleUserId, string email, string name, string profilePicture, string phoneNumber, string country, DateTime createdAt, bool verified, bool locked, CompanyInfo companyInfo, Education education, bool businessAccount, string externalLink, bool isEnrolled)
        {
            FBaseUserId = fBaseUserId;
            FBUserId = fBUserId;
            GoogleUserId = googleUserId;
            Email = email;
            Name = name;
            ProfilePicture = profilePicture;
            PhoneNumber = phoneNumber;
            Country = country;
            CreatedAt = createdAt;
            Verified = verified;
            Locked = locked;
            CompanyInfo = companyInfo;
            Education = education;
            BusinessAccount = businessAccount;
            ExternalLink = externalLink;
            IsEnrolled = isEnrolled;
            LinkRefs();
        }

        // used for registering with email
        public User(string fbaseUserId, string username, string email, string name)
        {
            FBaseUserId = fbaseUserId;
            FBUserId = null;
            GoogleUserId = null;
            Username = username;
            Email = email;
            Name = name;
            Init();
        }

        // used for registering with facebook
        public User(FBData data)
        {
            Email = data.Email;
            Name = data.Name;
            FBUserId = data.Id;
            FBaseUserId = null;
            GoogleUserId = null;
            ProfilePicture = data.Picture.Data.Url;
            Init();
        }

        // used for registering with google
        public User(GoogleData data)
        {
            Email = data.Email;
            Name = data.Name;
            GoogleUserId = data.Id;
            ProfilePicture = data.Picture;
            Init();
        }

        // used for updating User entity from UserDto
        public User(UserDto user)
        {
            BusinessAccount = user.BusinessAccount;
            CompanyInfo = user.CompanyInfo;
            Country = user.Country;
            CreatedAt = user.CreatedAt;
            Education = user.Education;
            Email = user.Email;
            ExternalLink = user.ExternalLink;
            FBaseUserId = user.FBaseUserId;
            FBUserId = user.FBUserId;
            GoogleUserId = user.GoogleUserId;
            LastLoggedIn = user.LastLoggedIn;
            Locked = user.Locked;
            ModifiedAt = user.ModifiedAt;
            Name = user.Name;
            PhoneNumber = user.PhoneNumber;
            ProfilePicture = user.ProfilePicture;
            Role = user.Role;
            RoleRef = new RefV(user.Role.Id);
            Verified = user.Verified;
            IsEnrolled = user.IsEnrolled;
            LinkRefs();
        }

        protected override void SetDefaults()
        {
            CreatedAt = DateTime.Now;
            RoleRef = GetQueryResult(
                FaunaDbClient.GetRef(DBCollections.ROLE, ((int)Roles.USER).ToString())
            );
            ModifiedAt = null;
            PhoneNumber = "";
            LastLoggedIn = null;
            Country = null;
            Locked = false;
            Education = new Education();
            CompanyInfo = null;
            IsEnrolled = false;
        }

        protected override void LinkRefs()
        {
            Role = ConvertValueToType<Role>(
                GetQueryResult(FaunaDbClient.GetDocument(DBCollections.ROLE, RoleRef.Id)
            ));
        }
    }
}
