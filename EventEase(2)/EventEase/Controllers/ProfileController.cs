using EventEase.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEase.Models;

namespace EventEase.Controllers
{
    public class ProfileController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;


        public ProfileController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var reservedEvents = new List<Event>();
            var organizedEvents = new List<Event>();

            if (userRoles.Contains("User"))
            {
                reservedEvents = _context.UserEvents
                    .Include(ue => ue.Event)
                    .Where(ue => ue.UserId == user.Id)
                    .Select(ue => ue.Event)
                    .ToList();
            }
            else if (userRoles.Contains("Organizer"))
            {
                organizedEvents = _context.UserEvents
                   .Include(ue => ue.Event)
                   .Where(ue => ue.UserId == user.Id)
                   .Select(ue => ue.Event)
                   .ToList();

            }

            var model = new ProfileViewModel
            {
                User = user,
                ReservedEvents = reservedEvents,
                OrganizedEvents = organizedEvents
            };

            return View(model);
        }
    }
}
