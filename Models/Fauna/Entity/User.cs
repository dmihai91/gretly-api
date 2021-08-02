using System;
using FaunaDB.Types;
using Gretly.Constants;
using Gretly.Dto;
using Gretly.Utils;
using Newtonsoft.Json;

namespace Gretly.Models
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

        [FaunaField("type")]
        public int Type { get; set; } = (int)ProfileType.SELLER;

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

        [FaunaIgnoreAttribute]
        public Models.Role Role { get; set; }

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

        [FaunaConstructor]
        public User(string fBaseUserId, string fBUserId, string googleUserId, string email, string name, string profilePicture, int type, string phoneNumber, string country, DateTime createdAt, RefV roleRef, bool verified, bool locked, CompanyInfo companyInfo, Education education, bool businessAccount, string externalLink)
        {
            FBaseUserId = fBaseUserId;
            FBUserId = fBUserId;
            GoogleUserId = googleUserId;
            Email = email;
            Name = name;
            ProfilePicture = profilePicture;
            Type = type;
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
            this.LinkRefs();
        }

        // used by json deserializer
        [JsonConstructor]
        public User(string fBaseUserId, string fBUserId, string googleUserId, string email, string name, string profilePicture, int type, string phoneNumber, string country, DateTime createdAt, bool verified, bool locked, CompanyInfo companyInfo, Education education, bool businessAccount, string externalLink)
        {
            FBaseUserId = fBaseUserId;
            FBUserId = fBUserId;
            GoogleUserId = googleUserId;
            Email = email;
            Name = name;
            ProfilePicture = profilePicture;
            Type = type;
            PhoneNumber = phoneNumber;
            Country = country;
            CreatedAt = createdAt;
            Verified = verified;
            Locked = locked;
            CompanyInfo = companyInfo;
            Education = education;
            BusinessAccount = businessAccount;
            ExternalLink = externalLink;
            RoleRef = this.GetQueryResult<RefV>(
               FaunaDbClient.GetRef(DBCollections.ROLE, ((int)Roles.SELLER).ToString())
            );
            this.LinkRefs();
        }

        // used for registering with email
        public User(string fbaseUserId, string username, string email, string name)
        {
            FBaseUserId = fbaseUserId;
            Username = username;
            Email = email;
            Name = name;
            this.Init();
        }

        // used for registering with facebook
        public User(FBData data)
        {
            Email = data.Email;
            Name = data.Name;
            FBUserId = data.Id;
            ProfilePicture = data.Picture.Data.Url;
            this.Init();
        }

        // used for registering with google
        public User(GoogleData data)
        {
            Email = data.Email;
            Name = data.Name;
            GoogleUserId = data.Id;
            ProfilePicture = data.Picture;
            this.Init();
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
            Type = (int)user.Type;
            Verified = user.Verified;
            this.LinkRefs();
        }

        protected override void SetDefaults()
        {
            CreatedAt = System.DateTime.Now;
            RoleRef = this.GetQueryResult<RefV>(
                FaunaDbClient.GetRef(DBCollections.ROLE, ((int)Roles.SELLER).ToString())
            );
        }

        protected override void LinkRefs()
        {
            Role = this.ConvertValueToType<Role>(
                this.GetQueryResult<Value>(FaunaDbClient.GetDocument(DBCollections.ROLE, this.RoleRef.Id)
            ));
        }
    }
}
