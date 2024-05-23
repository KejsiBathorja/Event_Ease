using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Location { get; set; }
        public DateTime Date { get; set; }
        public string? Discription { get; set; }

        [Display(Name = "Category Name")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public string? Image { get; set; } = String.Empty;

        public int OrganizerId { get; set; }
        public virtual Organizer Organizer { get; set; }

        [NotMapped]
        [DataType(DataType.Upload)]
        [Display(Name = "Upload Product Image")]
        [FileExtensions(Extensions = "jpg")]
        [Required]
        public IFormFile ImageFile { get; set; }
        

        public int TotalTickets { get; set; }
        public int AvailableTickets { get; set; }

        [Required]
        [Display(Name = "Price in Euro")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public Event()
        {
            AvailableTickets = TotalTickets; // Inicializon AvailableTickets me vlerën e TotalTickets
        }


    }
}
