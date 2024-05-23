using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using EventEase.Data;
using EventEase.Models;
using Microsoft.CodeAnalysis;
using Microsoft.AspNetCore.Identity;
using EventEase.Controllers;



namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RezervationService _reservationService;
        private readonly UserManager<IdentityUser> _userManager;

        public EventsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment, RezervationService reservationService, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
            _reservationService = reservationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var events = await _context.Event.Include(e => e.Category).ToListAsync();
            return View(events);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [Authorize(Roles = "Organizer")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName");
            ViewData["OrganizerId"] = new SelectList(_context.Organizer, "Id", "Name");
            return View();
        }

        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Discription,Date,CategoryId,OrganizerId,Image,ImageFile,TotalTickets,AvailableTickets,Price")] Event @event)
        {
            //if (ModelState.IsValid)
            {
                if (@event.ImageFile != null && @event.ImageFile.Length > 0)
                {
                    var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(@event.ImageFile.FileName);
                    var filePath = Path.Combine(uploadsDir, fileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await @event.ImageFile.CopyToAsync(fileStream);
                    }

                    @event.Image = fileName;
                }

                _context.Add(@event);
                await _context.SaveChangesAsync();

                // Përditëso vlerën e AvailableTickets me vlerën e TotalTickets
                @event.AvailableTickets = @event.TotalTickets;
                _context.Update(@event);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["OrganizerId"] = new SelectList(_context.Organizer, "Id", "Name", @event.OrganizerId);
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", @event.CategoryId);
            return View(@event);
           
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", @event.CategoryId);
            ViewData["OrganizerId"] = new SelectList(_context.Organizer, "Id", "Name", @event.OrganizerId);
            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Location,Discription,CategoryId,Image,ImageFile")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

          //  if (ModelState.IsValid)
            {
                try
                {
                    if (@event.ImageFile != null && @event.ImageFile.Length > 0)
                    {
                        var uploadsDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                        var fileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(@event.ImageFile.FileName);
                        var filePath = Path.Combine(uploadsDir, fileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await @event.ImageFile.CopyToAsync(fileStream);
                        }

                        @event.Image = fileName; // Update the image property with the new file name
                    }

                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            // If the model state is not valid, return the view with the event data
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName", @event.CategoryId);
            ViewData["OrganizerId"] = new SelectList(_context.Organizer, "Id", "Name", @event.OrganizerId);
            return View(@event);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Event
                .Include(e => e.Category)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Event.FindAsync(id);
            _context.Event.Remove(@event);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Event.Any(e => e.Id == id);
        }

        public IActionResult Event(string searchTerm, int? categoryFilter)
        {
            var categories = _context.Category
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName
                })
                .ToList();

            ViewBag.Categories = categories;

            var events = _context.Event
                .Include(e => e.Category)
                .Where(e =>
                    (string.IsNullOrEmpty(searchTerm) || e.Discription.Contains(searchTerm)) &&
                    (!categoryFilter.HasValue || e.CategoryId == categoryFilter)
                )
                .ToList();

            if (events.Count == 0)
            {
                ViewBag.NoItemsMessage = "No events found.";
            }

            ViewBag.CategoryFilter = categoryFilter ?? 0;

            return View(events);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> ReserveTicket(int id)
        {
            var @event = await _context.Event.FindAsync(id);

            if (@event == null)
            {
                return NotFound();
            }

            if (@event.AvailableTickets <= 0)
            {
                ViewBag.ErrorMessage = "Nuk ka më vende të lira për këtë event.";
                return View("Error");
            }

            return View(@event);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReservation(ReservationModel reservationModel)
        {
            if (ModelState.IsValid)
            {
                var @event = await _context.Event.FindAsync(reservationModel.EventId);

                if (@event == null)
                {
                    return NotFound();
                }

                if (@event.AvailableTickets < reservationModel.NumTickets)
                {
                    ViewBag.ErrorMessage = "Numri i biletave të zgjedhura është më i madh se biletat e disponueshme.";
                    return View("Error");
                }

                decimal totalAmount = reservationModel.NumTickets * @event.Price;

                var reservationSuccessful = await _reservationService.ReserveTicket(@event, reservationModel.UserEmail, reservationModel.NumTickets, totalAmount);

                if (reservationSuccessful)
                {

                    @event.AvailableTickets -= reservationModel.NumTickets;
                    _context.Update(@event);
                    await _context.SaveChangesAsync();

                    ViewBag.SuccessMessage = "Rezervimi u krye me sukses!";
                    return View("Success");
                }
                else
                {
                    ViewBag.ErrorMessage = "Gabim gjatë rezervimit të biletës.";
                    return View("Error");
                }
            }

            return View(); // Ktheje view-në për të shfaqur formën e rezervimit
        }



    }
}

