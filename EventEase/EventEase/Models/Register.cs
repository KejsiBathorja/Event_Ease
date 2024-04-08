using System.ComponentModel.DataAnnotations;

namespace EventEase.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Please enter the First Name")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Please enter the Last Name")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Please enter the E-Mail")]
        public string E_mail { get; set; }

        [Required(ErrorMessage = "Please enter the Username")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Please enter the Password")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Please ri-enter the Password")]
        public string ConfirmPassword { get; set; }
        

    }
}
