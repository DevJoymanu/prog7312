using Microsoft.AspNetCore.Mvc;
using prog7312_st10161149_part1.Models;
using prog7312_st10161149_part1.Services;

namespace prog7312_st10161149_part1.Controllers
{
    public class EventsController : Controller
    {
        private readonly IEventService _eventService;
        private readonly ILogger<EventsController> _logger;

        public EventsController(IEventService eventService, ILogger<EventsController> logger)
        {
            _eventService = eventService;
            _logger = logger;
        }

        // GET: Events
        public async Task<IActionResult> Index(string searchTerm, string category, DateTime? startDate)
        {
            IEnumerable<Event> events;

            if (!string.IsNullOrEmpty(searchTerm) || !string.IsNullOrEmpty(category) || startDate.HasValue)
            {
                events = await _eventService.SearchEventsAsync(searchTerm, category, startDate);
                ViewBag.SearchPerformed = true;
                ViewBag.SearchTerm = searchTerm;
                ViewBag.SelectedCategory = category;
                ViewBag.StartDate = startDate;
            }
            else
            {
                events = await _eventService.GetAllAsync();
            }

            ViewBag.Categories = EventCategories.Categories;
            
            // Get category statistics
            var stats = await _eventService.GetCategoryStatisticsAsync();
            ViewBag.CategoryStats = stats;

            // Get recently viewed events (Stack)
            var recentlyViewed = await _eventService.GetRecentlyViewedEventsAsync();
            ViewBag.RecentlyViewed = recentlyViewed.Take(5).ToList();

            return View(events);
        }

        // GET: Events/Details/
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _eventService.GetByIdAsync(id.Value);
            if (eventItem == null) return NotFound();

            // Track as recently viewed
            await _eventService.AddToRecentlyViewedAsync(id.Value);

            // Recommend based on popularity (excluding current event)
            var mostViewed = await _eventService.GetMostViewedEventsAsync();
            ViewBag.RecommendedEvents = mostViewed
                .Where(e => e.Id != id.Value)
                .Take(3)
                .ToList();

            return View(eventItem);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            ViewBag.Categories = EventCategories.Categories;
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Event eventItem)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = EventCategories.Categories;
                return View(eventItem);
            }

            try
            {
                var createdEvent = await _eventService.CreateAsync(eventItem); // Get event with ID
                TempData["Success"] = "Event created successfully!";
                return RedirectToAction(nameof(Details), new { id = createdEvent.Id }); // Redirect to Details
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating event");
                ModelState.AddModelError("", "An error occurred while creating the event. Please try again.");
                ViewBag.Categories = EventCategories.Categories;
                return View(eventItem);
            }
        }

        // GET: Events/Edit/
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _eventService.GetByIdAsync(id.Value);
            if (eventItem == null) return NotFound();

            ViewBag.Categories = EventCategories.Categories;
            return View(eventItem);
        }

        // POST: Events/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Event eventItem)
        {
            if (id != eventItem.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = EventCategories.Categories;
                return View(eventItem);
            }

            try
            {
                var updated = await _eventService.UpdateAsync(eventItem);
                if (updated == null) return NotFound();

                TempData["Success"] = "Event updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating event");
                ModelState.AddModelError("", "An error occurred while updating the event. Please try again.");
                ViewBag.Categories = EventCategories.Categories;
                return View(eventItem);
            }
        }

        // GET: Events/Delete/
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var eventItem = await _eventService.GetByIdAsync(id.Value);
            if (eventItem == null) return NotFound();

            return View(eventItem);
        }

        // POST: Events/Delete/
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _eventService.DeleteAsync(id);
                TempData["Success"] = "Event deleted successfully!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting event");
                TempData["Error"] = "An error occurred while deleting the event.";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Events/Calendar
        public async Task<IActionResult> Calendar()
        {
            var eventsByDate = await _eventService.GetEventsByDateAsync();
            return View(eventsByDate);
        }
    }
}