using System.ComponentModel.DataAnnotations;

namespace prog7312_st10161149_part1.Models
{
    public class ReportIssueViewModel
        {
            [Required(ErrorMessage = "Location is required")]
            [Display(Name = "Issue Location")]
            [StringLength(200, ErrorMessage = "Location cannot exceed 200 characters")]
            public string Location { get; set; } = string.Empty;

            [Required(ErrorMessage = "Please select a category")]
            [Display(Name = "Issue Category")]
            public string Category { get; set; } = string.Empty;

            [Required(ErrorMessage = "Description is required")]
            [Display(Name = "Issue Description")]
            [StringLength(1000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 1000 characters")]
            public string Description { get; set; } = string.Empty;

            [Display(Name = "Your Name (Optional)")]
            [StringLength(100)]
            public string? ReporterName { get; set; }

            [Display(Name = "Email (Optional)")]
            [EmailAddress(ErrorMessage = "Please enter a valid email address")]
            public string? ReporterEmail { get; set; }

            [Display(Name = "Photo (Optional)")]
            public IFormFile? Photo { get; set; }
        }
    }