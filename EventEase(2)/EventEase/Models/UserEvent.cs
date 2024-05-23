using Microsoft.AspNetCore.Identity;

namespace EventEase.Models
{
    public class UserEvent
    {
        public string UserId { get; set; }
        public int EventId { get; set; }
        public IdentityUser User { get; set; }
        public Event Event { get; set; }
    }
}
