using EventEase.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EventEase.Models;
using EventEase.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace EventEase.Controllers
{
    public class RoleAdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> UserManager;
        public RoleManager<IdentityRole> RoleManager;
        public IEnumerable<IdentityRole> Roles { get; set; }

        public RoleAdminController(UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager, ApplicationDbContext context)
        {
            this.UserManager = userManager;
            this.RoleManager = roleManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var users = UserManager.Users.Select(user => new UserDetailsViewModel()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = string.Join(",", UserManager.GetRolesAsync(user).Result.ToArray())
            }).ToList();
            return View(users);
        }

        [HttpGet]
        public async Task<IActionResult> ShowEdit(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (id == null)
            {
                return BadRequest("User not found");
            }
            var model = new UserDetailsViewModel
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName
            };
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(UserDetailsViewModel model)
        {
            var user = await UserManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return BadRequest("User not found" + model.Id);
            }
            else
            {
                user.Email = model.Email;
                user.UserName = model.UserName;
                var result = await UserManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                return View(model);
            }
        }
        public async Task<IActionResult> Delete(string id)
        {
            IdentityUser user = await UserManager.FindByIdAsync(id);
            if (user != null)
            {
                await UserManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> Create(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }
            var model = new UserDetailsViewModel
            {
                Id = user.Id,
                Email = user.Email
            };
            var roles = RoleManager.Roles;
            ViewBag.Roles = new SelectList(roles.ToList(), "Id", "Name");
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUserRole(UserDetailsViewModel u)
        {
            var user = await UserManager.FindByEmailAsync(u.Email);
            if (user == null)
            {
                return BadRequest("User does not exist" + u.Email);
            }

            var name = Convert.ToString(await RoleManager.FindByIdAsync(u.Role));
            if (name == null)
            {
                return BadRequest("Role does not exist" + u.Role);
            }

            await UserManager.AddToRoleAsync(user, name);
            return RedirectToAction("Index");
        }
        [HttpGet]
        public async Task<ActionResult> RemoveUserRole(string id)
        {
            var user = await UserManager.FindByIdAsync(id);
            if (user == null)
            {
                return BadRequest("User not found");
            }

            var roles = await UserManager.GetRolesAsync(user);

            ViewBag.Roles = new SelectList(await RoleManager.Roles.ToListAsync(), "Name", "Name");

            var model = new UserDetailsViewModel
            {
                Id = user.Id,
                Email = user.Email
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RemoveUserRole(UserDetailsViewModel u)
        {
            var user = await UserManager.FindByIdAsync(u.Id);
            if (user == null)
            {
                return BadRequest("User does not exist" + u.Email);
            }
            if (string.IsNullOrEmpty(u.Role))
            {
                return BadRequest("Role is null or empty");
            }

            var result = await UserManager.RemoveFromRoleAsync(user, u.Role);
            if (result.Succeeded)
            {
                // Ruaj ndryshimet
                await _context.SaveChangesAsync();

                return RedirectToAction("Index"); // Shto këtë rresht për redirektim pasi të ketë sukses
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("RemoveUserRole", u);
        }
    }
}
