using FrontendApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using System.IO;

namespace FrontendApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IcristatDbContext _context;
        private readonly ILogger<IndexModel> _logger;

        static IndexModel()
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        }

        public IndexModel(IcristatDbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public float TempMin { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
        public float TempMax { get; set; } = 50;
        [BindProperty(SupportsGet = true)]
        public float HumidityMin { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
        public float HumidityMax { get; set; } = 100;
        [BindProperty(SupportsGet = true)]
        public float RainfallMin { get; set; } = 0;
        [BindProperty(SupportsGet = true)]
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

        public async Task<IActionResult> OnPostExportToExcelAsync([FromBody] FilterModel filters)
        {
            try
            {
                float epsilon = 0.01f;
                var filteredAccessionIdsQuery = _context.Characterizations
                    .Where(c =>
                        c.Temperature != null && c.Temperature >= (filters.TempMin - epsilon) && c.Temperature <= (filters.TempMax + epsilon) &&
                        c.Humidity != null && c.Humidity >= (filters.HumidityMin - epsilon) && c.Humidity <= (filters.HumidityMax + epsilon) &&
                        c.Rainfall != null && c.Rainfall >= (filters.RainfallMin - epsilon) && c.Rainfall <= (filters.RainfallMax + epsilon)
                    )
                    .Select(c => c.ICRISATAccessionIdentifier)
                    .Distinct();

                var passportData = await _context.PassportData
                    .Where(p => filteredAccessionIdsQuery.Contains(p.ICRISATAccessionIdentifier))
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

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Passport Data");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "ICRISAT Accession Identifier";
                    worksheet.Cells[1, 2].Value = "Accession Identifier";
                    worksheet.Cells[1, 3].Value = "Crop";
                    worksheet.Cells[1, 4].Value = "DOI";
                    worksheet.Cells[1, 5].Value = "Local Name";
                    worksheet.Cells[1, 6].Value = "Genus";
                    worksheet.Cells[1, 7].Value = "Species";

                    // Style the header row
                    using (var range = worksheet.Cells[1, 1, 1, 7])
                    {
                        range.Style.Font.Bold = true;
                        range.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }

                    // Add data
                    int row = 2;
                    foreach (var item in passportData)
                    {
                        worksheet.Cells[row, 1].Value = item.ICRISATAccessionIdentifier;
                        worksheet.Cells[row, 2].Value = item.AccessionIdentifier;
                        worksheet.Cells[row, 3].Value = item.Crop;
                        worksheet.Cells[row, 4].Value = item.DOI;
                        worksheet.Cells[row, 5].Value = item.LocalName;
                        worksheet.Cells[row, 6].Value = item.Genus;
                        worksheet.Cells[row, 7].Value = item.Species;
                        row++;
                    }

                    // Auto-fit columns
                    worksheet.Cells[worksheet.Dimension.Address].AutoFitColumns();

                    // Generate the Excel file
                    var content = package.GetAsByteArray();
                    var contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    var fileName = $"ICRISAT_Data_Export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                    Response.Headers.Add("Content-Disposition", $"attachment; filename={fileName}");
                    return File(content, contentType, fileName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting data to Excel");
                return BadRequest(new { error = "Failed to export data to Excel" });
            }
        }

        public class FilterModel
        {
            public float TempMin { get; set; }
            public float TempMax { get; set; }
            public float HumidityMin { get; set; }
            public float HumidityMax { get; set; }
            public float RainfallMin { get; set; }
            public float RainfallMax { get; set; }
        }
    }
}