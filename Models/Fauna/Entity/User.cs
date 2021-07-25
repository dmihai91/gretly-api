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
            this.FBaseUserId = fBaseUserId;
            this.FBUserId = fBUserId;
            this.GoogleUserId = googleUserId;
            this.Email = email;
            this.Name = name;
            this.ProfilePicture = profilePicture;
            this.Type = type;
            this.PhoneNumber = phoneNumber;
            this.Country = country;
            this.CreatedAt = createdAt;
            this.RoleRef = roleRef;
            this.Verified = verified;
            this.Locked = locked;
            this.CompanyInfo = companyInfo;
            this.Education = education;
            this.BusinessAccount = businessAccount;
            this.ExternalLink = externalLink;
            this.LinkRefs();
        }

        // used by json deserializer
        [JsonConstructor]
        public User(string fBaseUserId, string fBUserId, string googleUserId, string email, string name, string profilePicture, int type, string phoneNumber, string country, DateTime createdAt, bool verified, bool locked, CompanyInfo companyInfo, Education education, bool businessAccount, string externalLink)
        {
            this.FBaseUserId = fBaseUserId;
            this.FBUserId = fBUserId;
            this.GoogleUserId = googleUserId;
            this.Email = email;
            this.Name = name;
            this.ProfilePicture = profilePicture;
            this.Type = type;
            this.PhoneNumber = phoneNumber;
            this.Country = country;
            this.CreatedAt = createdAt;
            this.Verified = verified;
            this.Locked = locked;
            this.CompanyInfo = companyInfo;
            this.Education = education;
            this.BusinessAccount = businessAccount;
            this.ExternalLink = externalLink;
            this.RoleRef = this.GetQueryResult<RefV>(
               FaunaDbClient.GetRef(DBCollections.ROLE, ((int)Roles.SELLER).ToString())
            );
            this.LinkRefs();
        }

        // used for registering with email
        public User(string fbaseUserId, string username, string email, string name)
        {
            this.FBaseUserId = fbaseUserId;
            this.Username = username;
            this.Email = email;
            this.Name = name;
            this.Init();
        }

        // used for registering with facebook
        public User(FBData data)
        {
            this.Email = data.Email;
            this.Name = data.Name;
            this.FBUserId = data.Id;
            this.ProfilePicture = data.Picture.Data.Url;
            this.Init();
        }

        // used for registering with google
        public User(GoogleData data)
        {
            this.Email = data.Email;
            this.Name = data.Name;
            this.GoogleUserId = data.Id;
            this.ProfilePicture = data.Picture;
            this.Init();
        }

        // used for updating User entity from UserDto
        public User(UserDto user)
        {
            this.BusinessAccount = user.BusinessAccount;
            this.CompanyInfo = user.CompanyInfo;
            this.Country = user.Country;
            this.CreatedAt = user.CreatedAt;
            this.Education = user.Education;
            this.Email = user.Email;
            this.ExternalLink = user.ExternalLink;
            this.FBaseUserId = user.FBaseUserId;
            this.FBUserId = user.FBUserId;
            this.GoogleUserId = user.GoogleUserId;
            this.LastLoggedIn = user.LastLoggedIn;
            this.Locked = user.Locked;
            this.ModifiedAt = user.ModifiedAt;
            this.Name = user.Name;
            this.PhoneNumber = user.PhoneNumber;
            this.ProfilePicture = user.ProfilePicture;
            this.Role = user.Role;
            this.RoleRef = new RefV(user.Role.Id);
            this.Type = (int)user.Type;
            this.Verified = user.Verified;
            this.LinkRefs();
        }

        protected override void SetDefaults()
        {
            this.CreatedAt = System.DateTime.Now;
            this.RoleRef = this.GetQueryResult<RefV>(
                FaunaDbClient.GetRef(DBCollections.ROLE, ((int)Roles.SELLER).ToString())
            );
        }

        protected override void LinkRefs()
        {
            this.Role = this.ConvertValueToType<Role>(
                this.GetQueryResult<Value>(FaunaDbClient.GetDocument(DBCollections.ROLE, this.RoleRef.Id)
            ));
        }
    }
}
