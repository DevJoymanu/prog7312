using System.Collections.Generic;

namespace prog7312_st10161149_part1.Models
{
    public static class EventCategories
    {
        public static readonly Dictionary<string, string> Categories = new Dictionary<string, string>
        {
            { "meeting", "Meeting" },
            { "maintenance", "Maintenance" },
            { "community", "Community" },
            { "cultural", "Cultural" },
            { "announcement", "Announcement" },
            { "sports", "Sports" },
            { "health", "Health" }
        };
    }
}
