using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class ReviewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [Display (Name = "Your Full Name")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid Email Address")]
        [Display(Name = "Your Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        [Display(Name = "Rate your experience in the EventEase")]
        public int Rating { get; set; }

        [Required(ErrorMessage = "Message is required")]
        [Display(Name = "Leave a message to explain the rating")]
        public string? Message { get; set; }
        [NotMapped]
        public DateTime DateTime { get; set; }
    }
}
