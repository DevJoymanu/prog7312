using prog7312_st10161149_part1.Models;

namespace prog7312_st10161149_part1.Services
{
    public interface IEventService
    {
        Task<IEnumerable<Event>> GetAllAsync();
        Task<Event?> GetByIdAsync(int id);
        Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm, string? category = null, DateTime? startDate = null);
        Task<Dictionary<string, List<Event>>> GetEventsByDateAsync();
        Task<Event> CreateAsync(Event eventItem);
        Task<Event?> UpdateAsync(Event eventItem);
        Task<bool> DeleteAsync(int id);
        Task<Dictionary<string, int>> GetCategoryStatisticsAsync();
        Task<IEnumerable<Event>> GetRecommendedEventsAsync(string userCategory);
        Task<Stack<Event>> GetRecentlyViewedEventsAsync();
        Task AddToRecentlyViewedAsync(int eventId); Task<IEnumerable<Event>> GetMostViewedEventsAsync();



    }

    public class EventService : IEventService
    {
        // Stack: For tracking recently viewed events (LIFO - Last In First Out)
        private readonly Stack<Event> _recentlyViewedStack;

        // Queue: For upcoming events processing (FIFO - First In First Out)
        private readonly Queue<Event> _upcomingEventsQueue;

        // Priority Queue using SortedDictionary: Events organized by date priority
        private readonly SortedDictionary<DateTime, List<Event>> _eventsByDatePriority;

        // Dictionary (Hash Table): Fast O(1) lookup by category
        private readonly Dictionary<string, HashSet<Event>> _eventsByCategory;

        // Sorted Dictionary: Events sorted by date and title for efficient searching
        private readonly SortedDictionary<string, Event> _sortedEventsByDate;

        // HashSets: For tracking unique categories and dates
        private readonly HashSet<string> _uniqueCategories;
        private readonly HashSet<DateTime> _uniqueDates;

        // Main event storage (List)
        private readonly List<Event> _events;
        private int _nextId = 1;

        public EventService()
        {
            _events = new List<Event>();
            _recentlyViewedStack = new Stack<Event>();
            _upcomingEventsQueue = new Queue<Event>();
            _eventsByDatePriority = new SortedDictionary<DateTime, List<Event>>();
            _eventsByCategory = new Dictionary<string, HashSet<Event>>();
            _sortedEventsByDate = new SortedDictionary<string, Event>();
            _uniqueCategories = new HashSet<string>();
            _uniqueDates = new HashSet<DateTime>();

            // Initialize category dictionary with all categories
            foreach (var category in EventCategories.Categories.Keys)
            {
                _eventsByCategory[category] = new HashSet<Event>();
            }

            SeedSampleData();
            UpdateUpcomingEventsQueue();
        }

        private void SeedSampleData()
        {
            var sampleEvents = new List<Event>
            {
                new Event
                {
                    Id = _nextId++,
                    Title = "Municipal Budget Public Meeting",
                    Description = "Join us for the annual municipal budget presentation and community input session. All residents are welcome to attend and voice their opinions on budget allocations.",
                    Category = "meeting",
                    EventDate = DateTime.Now.AddDays(7).Date.AddHours(18),
                    Location = "City Hall, Main Auditorium",
                    OrganizerName = "Municipal Finance Department",
                    ContactEmail = "finance@municipality.gov.za",
                    CreatedAt = DateTime.Now.AddDays(-10)
                },
                new Event
                {
                    Id = _nextId++,
                    Title = "Water Supply Maintenance Notice",
                    Description = "Scheduled water supply interruption for infrastructure upgrades. Affected areas include Zones A, B, and C. Water tankers will be available.",
                    Category = "maintenance",
                    EventDate = DateTime.Now.AddDays(3).Date.AddHours(6),
                    Location = "Zones A, B, C",
                    OrganizerName = "Water & Sanitation Department",
                    ContactEmail = "water@municipality.gov.za",
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new Event
                {
                    Id = _nextId++,
                    Title = "Community Clean-Up Day",
                    Description = "Join your neighbors for our monthly community clean-up initiative. Cleaning supplies and refreshments will be provided. Let's keep our community beautiful!",
                    Category = "community",
                    EventDate = DateTime.Now.AddDays(14).Date.AddHours(9),
                    Location = "Central Park",
                    OrganizerName = "Community Development",
                    ContactEmail = "community@municipality.gov.za",
                    CreatedAt = DateTime.Now.AddDays(-8)
                },
                new Event
                {
                    Id = _nextId++,
                    Title = "Heritage Day Cultural Festival",
                    Description = "Celebrate South Africa's diverse heritage with music, dance, traditional food, and cultural exhibitions. Free entry for all residents.",
                    Category = "cultural",
                    EventDate = DateTime.Now.AddDays(21).Date.AddHours(10),
                    Location = "Municipal Sports Grounds",
                    OrganizerName = "Arts & Culture Department",
                    ContactEmail = "culture@municipality.gov.za",
                    CreatedAt = DateTime.Now.AddDays(-15)
                },
                new Event
                {
                    Id = _nextId++,
                    Title = "Road Closure Announcement",
                    Description = "Main Street will be closed for road resurfacing from 6 AM to 6 PM. Alternative routes will be signposted. We apologize for any inconvenience.",
                    Category = "announcement",
                    EventDate = DateTime.Now.AddDays(2).Date.AddHours(6),
                    Location = "Main Street (Oak Ave to Elm St)",
                    OrganizerName = "Roads & Infrastructure",
                    ContactEmail = "roads@municipality.gov.za",
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new Event
                {
                    Id = _nextId++,
                    Title = "Youth Sports Tournament",
                    Description = "Annual inter-school sports tournament featuring soccer, netball, and athletics. Come support our local youth athletes!",
                    Category = "sports",
                    EventDate = DateTime.Now.AddDays(28).Date.AddHours(8),
                    Location = "Municipal Stadium",
                    OrganizerName = "Sports & Recreation",
                    ContactEmail = "sports@municipality.gov.za",
                    CreatedAt = DateTime.Now.AddDays(-12)
                },
                new Event
                {
                    Id = _nextId++,
                    Title = "Electricity Load Shedding Schedule Update",
                    Description = "Updated load shedding schedule for the next two weeks. Please check your zone's schedule and plan accordingly.",
                    Category = "announcement",
                    EventDate = DateTime.Now.AddDays(1).Date.AddHours(12),
                    Location = "All Municipal Areas",
                    OrganizerName = "Electricity Department",
                    ContactEmail = "electricity@municipality.gov.za",
                    CreatedAt = DateTime.Now.AddDays(-1)
                },
                new Event
                {
                    Id = _nextId++,
                    Title = "Free Health Screening Day",
                    Description = "Free health screenings including blood pressure, diabetes testing, and general wellness checks. No appointment necessary.",
                    Category = "community",
                    EventDate = DateTime.Now.AddDays(10).Date.AddHours(8),
                    Location = "Community Health Center",
                    OrganizerName = "Health Services",
                    ContactEmail = "health@municipality.gov.za",
                    CreatedAt = DateTime.Now.AddDays(-7)
                }
            };

            foreach (var evt in sampleEvents)
            {
                AddToDataStructures(evt);
                _events.Add(evt);
            }
        }

        // Add event to all data structures
        private void AddToDataStructures(Event evt)
        {
            // Add to priority queue (SortedDictionary by date)
            if (!_eventsByDatePriority.ContainsKey(evt.EventDate.Date))
            {
                _eventsByDatePriority[evt.EventDate.Date] = new List<Event>();
            }
            _eventsByDatePriority[evt.EventDate.Date].Add(evt);

            // Add to category hash table (Dictionary)
            if (_eventsByCategory.ContainsKey(evt.Category))
            {
                _eventsByCategory[evt.Category].Add(evt);
            }

            // Add to sorted dictionary (for searching)
            string sortKey = $"{evt.EventDate:yyyyMMddHHmmss}_{evt.Id}";
            _sortedEventsByDate[sortKey] = evt;

            // Add to sets
            _uniqueCategories.Add(evt.Category);
            _uniqueDates.Add(evt.EventDate.Date);
        }

        // Remove event from all data structures
        private void RemoveFromDataStructures(Event evt)
        {
            // Remove from priority queue
            if (_eventsByDatePriority.ContainsKey(evt.EventDate.Date))
            {
                _eventsByDatePriority[evt.EventDate.Date].Remove(evt);
                if (_eventsByDatePriority[evt.EventDate.Date].Count == 0)
                {
                    _eventsByDatePriority.Remove(evt.EventDate.Date);
                }
            }

            // Remove from category dictionary
            if (_eventsByCategory.ContainsKey(evt.Category))
            {
                _eventsByCategory[evt.Category].Remove(evt);
            }

            // Remove from sorted dictionary
            string sortKey = $"{evt.EventDate:yyyyMMddHHmmss}_{evt.Id}";
            _sortedEventsByDate.Remove(sortKey);
        }

        // Update the upcoming events queue (FIFO)
        private void UpdateUpcomingEventsQueue()
        {
            _upcomingEventsQueue.Clear();
            var upcomingEvents = _events
                .Where(e => e.EventDate >= DateTime.Now)
                .OrderBy(e => e.EventDate)
                .Take(10);

            foreach (var evt in upcomingEvents)
            {
                _upcomingEventsQueue.Enqueue(evt);
            }
        }

        public Task<IEnumerable<Event>> GetAllAsync()
        {
            var events = _events.OrderBy(e => e.EventDate).ToList();
            return Task.FromResult<IEnumerable<Event>>(events);
        }

        public Task<Event?> GetByIdAsync(int id)
        {
            var evt = _events.FirstOrDefault(e => e.Id == id);
            return Task.FromResult(evt);
        }

        // Search using Dictionary for category filtering
        public Task<IEnumerable<Event>> SearchEventsAsync(string searchTerm, string? category = null, DateTime? startDate = null)
        {
            IEnumerable<Event> results = _events;

            // Category dictionary for fast filtering
            if (!string.IsNullOrEmpty(category) && _eventsByCategory.ContainsKey(category))
            {
                results = _eventsByCategory[category];
            }

            // Text search
            if (!string.IsNullOrEmpty(searchTerm))
            {
                results = results.Where(e =>
                    e.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    e.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (e.Location != null && e.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // Use SortedDictionary for date range filtering
            if (startDate.HasValue)
            {
                results = results.Where(e => e.EventDate >= startDate.Value);
            }

            return Task.FromResult(results.OrderBy(e => e.EventDate).AsEnumerable());
        }

        // Get events grouped by date using SortedDictionary
        public Task<Dictionary<string, List<Event>>> GetEventsByDateAsync()
        {
            var groupedEvents = new Dictionary<string, List<Event>>();

            foreach (var dateGroup in _eventsByDatePriority)
            {
                string dateKey = dateGroup.Key.ToString("yyyy-MM-dd");
                groupedEvents[dateKey] = dateGroup.Value.OrderBy(e => e.EventDate).ToList();
            }

            return Task.FromResult(groupedEvents);
        }

        public Task<Event> CreateAsync(Event eventItem)
        {
            eventItem.Id = _nextId++;
            eventItem.CreatedAt = DateTime.Now;
            _events.Add(eventItem);
            AddToDataStructures(eventItem);
            UpdateUpcomingEventsQueue();
            return Task.FromResult(eventItem);
        }

        public Task<Event?> UpdateAsync(Event eventItem)
        {
            var existingEvent = _events.FirstOrDefault(e => e.Id == eventItem.Id);
            if (existingEvent == null)
                return Task.FromResult<Event?>(null);

            // Remove from data structures with old data
            RemoveFromDataStructures(existingEvent);

            // Update properties
            existingEvent.Title = eventItem.Title;
            existingEvent.Description = eventItem.Description;
            existingEvent.Category = eventItem.Category;
            existingEvent.EventDate = eventItem.EventDate;
            existingEvent.Location = eventItem.Location;
            existingEvent.OrganizerName = eventItem.OrganizerName;
            existingEvent.ContactEmail = eventItem.ContactEmail;

            // Re-add to data structures with new data
            AddToDataStructures(existingEvent);
            UpdateUpcomingEventsQueue();

            return Task.FromResult<Event?>(existingEvent);
        }

        public Task<bool> DeleteAsync(int id)
        {
            var evt = _events.FirstOrDefault(e => e.Id == id);
            if (evt == null)
                return Task.FromResult(false);

            RemoveFromDataStructures(evt);
            _events.Remove(evt);
            UpdateUpcomingEventsQueue();

            return Task.FromResult(true);
        }

        // Get category statistics using Dictionary
        public Task<Dictionary<string, int>> GetCategoryStatisticsAsync()
        {
            var stats = new Dictionary<string, int>();

            foreach (var category in _eventsByCategory)
            {
                stats[category.Key] = category.Value.Count;
            }

            return Task.FromResult(stats);
        }

        public Task<IEnumerable<Event>> GetMostViewedEventsAsync()
        {
            var mostViewed = _events
                .Where(e => e.EventDate >= DateTime.Now) 
                .OrderByDescending(e => e.ViewCount)
                .Take(10); 

            return Task.FromResult<IEnumerable<Event>>(mostViewed);
        }



        // Recommendation engine using Dictionary for category-based suggestions
        public Task<IEnumerable<Event>> GetRecommendedEventsAsync(string userCategory)
        {
            var recommended = new List<Event>();

            // 1. Get top 3 most viewed events in the user's category (upcoming only)
            if (!string.IsNullOrEmpty(userCategory) && _eventsByCategory.ContainsKey(userCategory))
            {
                recommended.AddRange(
                    _eventsByCategory[userCategory]
                        .Where(e => e.EventDate >= DateTime.Now)
                        .OrderByDescending(e => e.ViewCount)  
                        .ThenBy(e => e.EventDate)             
                        .Take(3)
                );
            }

            // 2. If less than 3 recommended, fill with other popular upcoming events outside userCategory
            if (recommended.Count < 3)
            {
                var additionalEvents = _events
                    .Where(e => e.EventDate >= DateTime.Now && !recommended.Contains(e))
                    .OrderByDescending(e => e.ViewCount)   
                    .ThenBy(e => e.EventDate)
                    .Take(3 - recommended.Count);

                recommended.AddRange(additionalEvents);
            }

            return Task.FromResult<IEnumerable<Event>>(recommended);
        }
        // Stack operations for recently viewed events
        public Task<Stack<Event>> GetRecentlyViewedEventsAsync()
        {
            return Task.FromResult(_recentlyViewedStack);
        }

        public Task AddToRecentlyViewedAsync(int eventId)
        {
            var evt = _events.FirstOrDefault(e => e.Id == eventId);
            if (evt != null)
            {
                // Remove if already in stack to avoid duplicates
                var tempStack = new Stack<Event>();
                while (_recentlyViewedStack.Count > 0)
                {
                    var item = _recentlyViewedStack.Pop();
                    if (item.Id != eventId)
                    {
                        tempStack.Push(item);
                    }
                }

                // Restore stack
                while (tempStack.Count > 0)
                {
                    _recentlyViewedStack.Push(tempStack.Pop());
                }

                // Add new item to top of stack
                _recentlyViewedStack.Push(evt);

                // Keep only last 10 viewed events
                if (_recentlyViewedStack.Count > 10)
                {
                    var temp = _recentlyViewedStack.ToList();
                    _recentlyViewedStack.Clear();
                    foreach (var item in temp.Take(10))
                    {
                        _recentlyViewedStack.Push(item);
                    }
                }

                // Increment view count
                evt.ViewCount++;
            }

            return Task.CompletedTask;
        }
    }
}

