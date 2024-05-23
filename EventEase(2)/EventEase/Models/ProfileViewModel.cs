using Microsoft.AspNetCore.Identity;

namespace EventEase.Models
{
    public class ProfileViewModel
    {
        public IdentityUser User { get; set; }
        public List<Event> ReservedEvents { get; set; }
        public List<Event> OrganizedEvents { get; set; }
    }
}
