using System.Runtime.Serialization;

namespace Gretly.Dto
{
    [DataContract]
    public class VatNumberValidatorResponse
    {
        [DataMember(Name = "isValid")]
        public bool IsValid { get; set; }

        [DataMember(Name = "name")]
        public string Name { get; set; }

        [DataMember(Name = "address")]
        public string Address { get; set; }
    }
}