using System;
using System.ComponentModel.DataAnnotations;

namespace prog7312_st10161149_part1.Models
{
    public class Event
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Title must be less than 100 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "Description must be less than 1000 characters.")]
        public string Description { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        [Display(Name = "Event Date")]
        public DateTime EventDate { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        [Display(Name = "Organizer Name")]
        [StringLength(100)]
        public string? OrganizerName { get; set; }

        [EmailAddress]
        [Display(Name = "Contact Email")]
        public string? ContactEmail { get; set; }

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Display(Name = "View Count")]
        public int ViewCount { get; set; } = 0;


    }
}
