using System.ComponentModel.DataAnnotations;

namespace Gretly.Models
{
    public class ApiSuccessResponse
    {
        [Display(Name = "status")]
        public string Status { get; set; }

        public ApiSuccessResponse(string status)
        {
            Status = status;
        }
    }
}