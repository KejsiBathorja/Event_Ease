using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class RoleViewModel
    {
        [Required]
        [Display(Name = "Role")]
        public string Name { get; set; }
    }
}
