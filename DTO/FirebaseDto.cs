using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace GretlyStudio.Dto
{
    [DataContract]
    public class FirebaseDto
    {
        [DataMember(Name = "user_id")]
        public string UserId { get; set; }

        [DataMember(Name = "email")]
        public string Email { get; set; }

        public static explicit operator FirebaseDto(JwtSecurityToken v)
        {
            var claims = v.Claims;
            Dictionary<string, string> fields = new Dictionary<string, string>();
            foreach (var claim in claims)
            {
                fields[claim.Type] = claim.Value;
            }
            return JsonConvert.DeserializeObject<FirebaseDto>(JsonConvert.SerializeObject(fields));
        }
    }
}