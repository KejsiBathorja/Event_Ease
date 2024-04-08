using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace EventEase.Data.EF
{
    public class Users
    {
        [Key]
        public int id { get; set; }
        [Required]
        public int LoginId { get; set; }

        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string E_mail { get; set; }
      
    }
}
