using FrontendApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontendApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IcristatDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(IcristatDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty]
        public float TempMin { get; set; } = 0;
        [BindProperty]
        public float TempMax { get; set; } = 50;
        [BindProperty]
        public float HumidityMin { get; set; } = 0;
        [BindProperty]
        public float HumidityMax { get; set; } = 100;
        [BindProperty]
        public float RainfallMin { get; set; } = 0;
        [BindProperty]
        public float RainfallMax { get; set; } = 200;

        [BindProperty(SupportsGet = true)]
        public int PageIndex { get; set; } = 1;

        public int PageSize { get; set; } = 100; // Number of records per page

        public int TotalRecords { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalRecords / PageSize);

        public IList<PassportData> PassportResults { get; set; } = new List<PassportData>();

        public async Task<IActionResult> OnPostAsync()
        {
            _logger.LogInformation($"Filtering with Temp: {TempMin}-{TempMax}, Humidity: {HumidityMin}-{HumidityMax}, Rainfall: {RainfallMin}-{RainfallMax}");

            try
            {
                // Query charecterstics table with filters
                //var filteredAccessionIdsQuery = _context.Characterizations
                //    .Where(c => (c.Temperature != null && (c.Temperature >= TempMin && c.Temperature <= TempMax)) &&
                //               (c.Humidity != null && (c.Humidity >= HumidityMin && c.Humidity <= HumidityMax)) &&
                //               (c.Rainfall != null && (c.Rainfall >= RainfallMin && c.Rainfall <= RainfallMax)))
                //    .Select(c => c.ICRISATAccessionIdentifier)
                //    .Distinct();

                float epsilon = 0.01f;

                var filteredAccessionIdsQuery = _context.Characterizations
                    .Where(c =>
                        c.Temperature != null && c.Temperature >= (TempMin - epsilon) && c.Temperature <= (TempMax + epsilon) &&
                        c.Humidity != null && c.Humidity >= (HumidityMin - epsilon) && c.Humidity <= (HumidityMax + epsilon) &&
                        c.Rainfall != null && c.Rainfall >= (RainfallMin - epsilon) && c.Rainfall <= (RainfallMax + epsilon)
                    )
                    .Select(c => c.ICRISATAccessionIdentifier)
                    .Distinct();
                _logger.LogInformation("Total characterizations: " + _context.Characterizations.Count());
                _logger.LogInformation("With non-null temperature: " + _context.Characterizations.Count(c => c.Temperature != null));
                _logger.LogInformation("With non-null humidity: " + _context.Characterizations.Count(c => c.Humidity != null));
                _logger.LogInformation("With non-null rainfall: " + _context.Characterizations.Count(c => c.Rainfall != null));


                TotalRecords = await filteredAccessionIdsQuery.CountAsync();
                _logger.LogInformation($"Found {TotalRecords} matching ICRISAT_accession_identifier values.");

                //Debugging
                //var temps = _context.Characterizations
                //    .Where(c => c.Temperature >= 1 && c.Temperature <= 28)
                //    .Select(c => c.Temperature)
                //    .Distinct()
                //    .OrderBy(c => c)
                //    .ToList();

                //_logger.LogInformation($"Matching temps: {string.Join(", ", temps)}");

                if (TotalRecords > 0)
                {
                    // Apply pagination to the filtered accession IDs
                    var filteredAccessionIds = await filteredAccessionIdsQuery
                        .Skip((PageIndex - 1) * PageSize)
                        .Take(PageSize)
                        .ToListAsync();

                    // Select only the specified fields from passport_data
                    PassportResults = await _context.PassportData
                        .Where(p => filteredAccessionIds.Contains(p.ICRISATAccessionIdentifier))
                        .Select(p => new PassportData
                        {
                            ICRISATAccessionIdentifier = p.ICRISATAccessionIdentifier,
                            AccessionIdentifier = p.AccessionIdentifier,
                            Crop = p.Crop,
                            DOI = p.DOI,
                            LocalName = p.LocalName,
                            Genus = p.Genus,
                            Species = p.Species
                        })
                        .ToListAsync();

                    _logger.LogInformation($"Retrieved {PassportResults.Count} passport records for page {PageIndex}.");
                }
                else
                {
                    _logger.LogInformation("No matching ICRISAT_accession_identifier values found.");
                    PassportResults = new List<PassportData>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering and retrieving passport data.");
                PassportResults = new List<PassportData>();
            }

            return Page();
        }

        public async Task<IActionResult> OnGetAsync()
        {
            // Reset filters and page index on GET
            TempMin = 0;
            TempMax = 50;
            HumidityMin = 0;
            HumidityMax = 100;
            RainfallMin = 0;
            RainfallMax = 200;
            PageIndex = 1;
            PassportResults = new List<PassportData>();
            return Page();
        }
    }
}