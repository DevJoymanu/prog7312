    using System.ComponentModel.DataAnnotations;

namespace prog7312_st10161149_part1.Models
{

    public class IssueReport
        {
            public int Id { get; set; }

            [Required]
            [Display(Name = "Issue Location")]
            [StringLength(200)]
            public string Location { get; set; }

            [Required]
            [Display(Name = "Issue Category")]
            public string Category { get; set; }

            [Required]
            [Display(Name = "Issue Description")]
            [StringLength(1000)]
            public string Description { get; set; }

            public DateTime CreatedAt { get; set; } = DateTime.Now;

            public IssueStatus Status { get; set; } = IssueStatus.Submitted;

            [StringLength(100)]
            public string? ReporterName { get; set; }

            [EmailAddress]
            public string? ReporterEmail { get; set; }

            public string? PhotoPath { get; set; }
        }

        public enum IssueStatus
        {
            Submitted,
            InProgress,
            Resolved,
            Closed
        }

        public static class IssueCategories
        {
            public static readonly Dictionary<string, string> Categories = new()
        {
            { "roads", "🛣️ Roads & Infrastructure" },
            { "water", "💧 Water & Sanitation" },
            { "electricity", "⚡ Electricity" },
            { "waste", "🗑️ Waste Management" },
            { "streetlights", "💡 Street Lighting" },
            { "parks", "🌳 Parks & Recreation" },
            { "other", "📝 Other" }
        };
        }
    }