using Microsoft.AspNetCore.Mvc;
using prog7312_st10161149_part1.Data;
using prog7312_st10161149_part1.Models;
using Microsoft.EntityFrameworkCore;
using prog7312_st10161149_part1.Services;

namespace prog7312_st10161149_part1.Controllers
{
    public class IssuesController : Controller
    {
        private readonly IIssueService _issueService;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<IssuesController> _logger;

        public IssuesController(IIssueService issueService, IWebHostEnvironment environment, ILogger<IssuesController> logger)
        {
            _issueService = issueService;
            _environment = environment;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var issues = await _issueService.GetAllAsync();
            return View(issues);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var issue = await _issueService.GetByIdAsync(id.Value);
            if (issue == null) return NotFound();

            return View(issue);
        }

        // GET: Issues/Create
        public IActionResult Create()
        {
            ViewBag.Categories = IssueCategories.Categories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReportIssueViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = IssueCategories.Categories;
                return View(model);
            }

            try
            {
                var issueReport = new IssueReport
                {
                    Location = model.Location,
                    Category = model.Category,
                    Description = model.Description,
                    ReporterName = model.ReporterName,
                    ReporterEmail = model.ReporterEmail,
                    Status = IssueStatus.Submitted
                };

                // Handle photo upload
                if (model.Photo != null && model.Photo.Length > 0)
                {
                    var uploadsPath = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsPath))
                        Directory.CreateDirectory(uploadsPath);

                    var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(model.Photo.FileName)}";
                    var filePath = Path.Combine(uploadsPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Photo.CopyToAsync(stream);
                    }

                    issueReport.PhotoPath = $"/uploads/{fileName}";
                }

                await _issueService.CreateAsync(issueReport);

                TempData["Success"] = "Thank you! Your issue has been submitted successfully.";
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating issue report");
                ModelState.AddModelError("", "An error occurred while submitting your report. Please try again.");
                ViewBag.Categories = IssueCategories.Categories;
                return View(model);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var issue = await _issueService.GetByIdAsync(id.Value);
            if (issue == null) return NotFound();

            ViewBag.Categories = IssueCategories.Categories;
            return View(issue);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Location,Category,Description,Status,ReporterName,ReporterEmail")] IssueReport issue)
        {
            if (id != issue.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var updatedIssue = await _issueService.UpdateAsync(issue);
                    if (updatedIssue == null) return NotFound();

                    TempData["Success"] = "Issue updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating issue report");
                    ModelState.AddModelError("", "An error occurred while updating the issue. Please try again.");
                }
            }

            ViewBag.Categories = IssueCategories.Categories;
            return View(issue);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var issue = await _issueService.GetByIdAsync(id.Value);
            if (issue == null) return NotFound();

            return View(issue);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var issue = await _issueService.GetByIdAsync(id);
                if (issue != null)
                {
                    // Delete associated photo file if exists
                    if (!string.IsNullOrEmpty(issue.PhotoPath))
                    {
                        var photoPath = Path.Combine(_environment.WebRootPath, issue.PhotoPath.TrimStart('/'));
                        if (System.IO.File.Exists(photoPath))
                        {
                            System.IO.File.Delete(photoPath);
                        }
                    }

                    await _issueService.DeleteAsync(id);
                    TempData["Success"] = "Issue deleted successfully.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting issue report");
                TempData["Error"] = "An error occurred while deleting the issue.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}