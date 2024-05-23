using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Organizer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Organizer Name")]
        public string Name { get; set; }

        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        public List<Event> Events { get; set; } 

    }
}
