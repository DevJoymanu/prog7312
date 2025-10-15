using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using prog7312_st10161149_part1.Data;
using prog7312_st10161149_part1.Models;
using prog7312_st10161149_part1.Services;

namespace prog7312_st10161149_part1.Controllers
{

    public class HomeController : Controller
    {
        private readonly IIssueService _issueService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IIssueService issueService, ILogger<HomeController> logger)
        {
            _issueService = issueService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            // Get recent issues for dashboard stats (optional)
            var recentIssues = await _issueService.GetAllAsync();
            ViewBag.TotalIssues = recentIssues.Count();
            ViewBag.PendingIssues = recentIssues.Count(i => i.Status == IssueStatus.Submitted || i.Status == IssueStatus.InProgress);
            ViewBag.ResolvedIssues = recentIssues.Count(i => i.Status == IssueStatus.Resolved || i.Status == IssueStatus.Closed);

            return View();
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
