using FaunaDB.Types;

namespace GretlyStudio.Models
{
    public class Education
    {
        [FaunaField("country")]
        public string Type { get; set; }

        [FaunaField("domain")]
        public string Domain { get; set; }

        [FaunaField("institution")]
        public string Institution { get; set; }
    }
}