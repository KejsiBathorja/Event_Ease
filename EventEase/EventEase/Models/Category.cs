using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        [StringLength(25)]
        public string CategoryName { get; set; } = null!;
    }
}
