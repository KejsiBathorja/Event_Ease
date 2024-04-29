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

namespace EventEase.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public EventsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Category, "CategoryId", "CategoryName");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Location,Discription,CategoryId,Image,ImageFile")] Event @event)
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
                return RedirectToAction(nameof(Index));
            }

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

    }
}