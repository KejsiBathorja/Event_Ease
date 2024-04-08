using System.ComponentModel.DataAnnotations;
using System.Data;
using EventEase.Enums;


namespace EventEase.Data.EF
{
    public class Login
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}
