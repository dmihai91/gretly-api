using System.Runtime.Serialization;

namespace GretlyStudio.Models
{
    public class FBPicture
    {
        [DataMember(Name = "width")]
        public int Width { get; set; }

        [DataMember(Name = "height")]
        public int Height { get; set; }

        [DataMember(Name = "is_silhouette")]
        public bool IsSilhouette { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}