using prog7312_st10161149_part1.Models;
using prog7312_st10161149_part1.Models;

namespace prog7312_st10161149_part1.Services
    {
    public interface IIssueService
        {
            Task<IEnumerable<IssueReport>> GetAllAsync();
            Task<IssueReport?> GetByIdAsync(int id);
            Task<IssueReport> CreateAsync(IssueReport issue);
            Task<IssueReport?> UpdateAsync(IssueReport issue);
            Task<bool> DeleteAsync(int id);
            Task<bool> ExistsAsync(int id);
        }

        public class IssueService : IIssueService
        {
            private readonly List<IssueReport> _issues;
            private int _nextId = 1;

            public IssueService()
            {
                _issues = new List<IssueReport>();

                // Add some sample data for demonstration
                SeedSampleData();
            }

            private void SeedSampleData()
            {
                var sampleIssues = new List<IssueReport>
            {
                new IssueReport
                {
                    Id = _nextId++,
                    Location = "Corner of Main Street and Oak Avenue, Cape Town",
                    Category = "roads",
                    Description = "Large pothole causing damage to vehicles. The hole is approximately 2 meters wide and quite deep, making it dangerous for cars and motorcycles.",
                    Status = IssueStatus.InProgress,
                    ReporterName = "John Smith",
                    ReporterEmail = "john.smith@email.com",
                    CreatedAt = DateTime.Now.AddDays(-5)
                },
                new IssueReport
                {
                    Id = _nextId++,
                    Location = "Nelson Mandela Boulevard, Johannesburg",
                    Category = "streetlights",
                    Description = "Street light has been out for over a week. The area is very dark at night, creating safety concerns for pedestrians.",
                    Status = IssueStatus.Submitted,
                    ReporterName = "Sarah Johnson",
                    ReporterEmail = "sarah.j@email.com",
                    CreatedAt = DateTime.Now.AddDays(-3)
                },
                new IssueReport
                {
                    Id = _nextId++,
                    Location = "Waterfront District, Durban",
                    Category = "waste",
                    Description = "Garbage bins have not been collected for 2 weeks. The area smells terrible and attracts pests.",
                    Status = IssueStatus.Resolved,
                    ReporterName = "Michael Brown",
                    CreatedAt = DateTime.Now.AddDays(-7)
                },
                new IssueReport
                {
                    Id = _nextId++,
                    Location = "Church Street, Pretoria",
                    Category = "water",
                    Description = "Water pipe burst underground, causing flooding on the sidewalk and road.",
                    Status = IssueStatus.InProgress,
                    ReporterName = "Lisa Adams",
                    ReporterEmail = "l.adams@email.com",
                    CreatedAt = DateTime.Now.AddDays(-2)
                },
                new IssueReport
                {
                    Id = _nextId++,
                    Location = "Green Point Park, Cape Town",
                    Category = "parks",
                    Description = "Playground equipment is broken and potentially dangerous for children. Swing set chains are rusted and one slide has a crack.",
                    Status = IssueStatus.Submitted,
                    ReporterName = "David Wilson",
                    CreatedAt = DateTime.Now.AddDays(-1)
                }
            };

                _issues.AddRange(sampleIssues);
            }

            public Task<IEnumerable<IssueReport>> GetAllAsync()
            {
                var issues = _issues.OrderByDescending(i => i.CreatedAt).ToList();
                return Task.FromResult<IEnumerable<IssueReport>>(issues);
            }

            public Task<IssueReport?> GetByIdAsync(int id)
            {
                var issue = _issues.FirstOrDefault(i => i.Id == id);
                return Task.FromResult(issue);
            }

            public Task<IssueReport> CreateAsync(IssueReport issue)
            {
                issue.Id = _nextId++;
                issue.CreatedAt = DateTime.Now;
                _issues.Add(issue);
                return Task.FromResult(issue);
            }

            public Task<IssueReport?> UpdateAsync(IssueReport issue)
            {
                var existingIssue = _issues.FirstOrDefault(i => i.Id == issue.Id);
                if (existingIssue == null)
                    return Task.FromResult<IssueReport?>(null);

                // Update properties
                existingIssue.Location = issue.Location;
                existingIssue.Category = issue.Category;
                existingIssue.Description = issue.Description;
                existingIssue.Status = issue.Status;
                existingIssue.ReporterName = issue.ReporterName;
                existingIssue.ReporterEmail = issue.ReporterEmail;

                return Task.FromResult<IssueReport?>(existingIssue);
            }

            public Task<bool> DeleteAsync(int id)
            {
                var issue = _issues.FirstOrDefault(i => i.Id == id);
                if (issue == null)
                    return Task.FromResult(false);

                _issues.Remove(issue);
                return Task.FromResult(true);
            }

            public Task<bool> ExistsAsync(int id)
            {
                var exists = _issues.Any(i => i.Id == id);
                return Task.FromResult(exists);
            }
        }
    }