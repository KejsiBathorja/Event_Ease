using EventEase.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using EventEase.Data;

namespace EventEase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;
            var upcomingEvents = await _context.Event
                .Include(e => e.Category)
                .Where(e => e.Date >= today)
                .OrderBy(e => e.Date)
                .Take(4)
                .ToListAsync();
            // Log the events for debugging
            foreach (var eventItem in upcomingEvents)
            {
                _logger.LogInformation($"Event: {eventItem.Name}, Date: {eventItem.Date}");
            }

            return View(upcomingEvents);
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
