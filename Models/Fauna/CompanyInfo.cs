using FaunaDB.Types;

namespace Gretly.Models
{
    public class CompanyInfo
    {
        [FaunaField("country")]
        public string Country { get; set; }

        [FaunaField("vatNumber")]
        public string VatNumber { get; set; }

        [FaunaField("address")]
        public string Address { get; set; }
    }
}