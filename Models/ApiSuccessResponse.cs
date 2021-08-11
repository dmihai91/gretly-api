using System.ComponentModel.DataAnnotations;

namespace GretlyStudio.Models
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