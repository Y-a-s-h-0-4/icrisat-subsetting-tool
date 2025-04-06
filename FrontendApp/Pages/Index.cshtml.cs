using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FrontendApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FrontendApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IcristatDbContext _context;

        public IndexModel(ILogger<IndexModel> logger, IcristatDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public List<string> RaceValues { get; set; } = new List<string>();

        public async Task OnGetAsync()
        {
            _logger.LogInformation("Starting retrieval of Race values from the Characterization table.");

            try
            {
                // Check if the database connection is working
                _logger.LogInformation($"Database connection state: {_context.Database.CanConnect()}");

                // Retrieve Race values from the Characterization table
                RaceValues = await _context.Characterizations
                    .Select(c => c.Race)
                    .ToListAsync();

                // Log the number of values retrieved
                _logger.LogInformation($"Retrieved {RaceValues.Count} Race values.");

                // Log the actual values (or a message if the list is empty)
                if (RaceValues.Any())
                {
                    _logger.LogInformation($"Race values: {string.Join(", ", RaceValues)}");
                }
                else
                {
                    _logger.LogInformation("No Race values found in the Characterization table.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving Race values from the Characterization table.");
                RaceValues = new List<string>();
            }
        }
    }
}