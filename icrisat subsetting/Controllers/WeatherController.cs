using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using WeatherAPI.Models;
using System.Threading.Tasks;
using System.Linq;
using WeatherAPI.Data;
using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace WeatherAPI.Controllers
{
    [Route("api/weather")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WeatherController> _logger;
        private readonly IConfiguration _configuration;

        public WeatherController(
            IHttpClientFactory httpClientFactory,
            ApplicationDbContext context,
            ILogger<WeatherController> logger,
            IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient();
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpPost("process-all")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ProcessAllRecords()
        {
            try
            {
                _logger.LogInformation("Starting to process all records");
                var results = new List<WeatherResponse>();
                var errors = new List<string>();

                // Get all records from passport_data table
                var passportRecords = await _context.PassportData.ToListAsync();
                _logger.LogInformation($"Found {passportRecords.Count} records to process");

                var totalRecords = passportRecords.Count;
                var processedRecords = 0;
                var successfulRecords = 0;
                var failedRecords = 0;

                foreach (var record in passportRecords)
                {
                    processedRecords++;
                    Console.WriteLine($"Processing record {processedRecords}/{totalRecords}: {record.ICRISAT_accession_identifier}");

                    try
                    {
                        if (string.IsNullOrEmpty(record.Latitude) || string.IsNullOrEmpty(record.Longitude))
                        {
                            var error = $"Missing coordinates for {record.ICRISAT_accession_identifier}";
                            _logger.LogWarning(error);
                            errors.Add(error);
                            failedRecords++;
                            continue;
                        }

                        // Convert string coordinates to double
                        if (!double.TryParse(record.Latitude, out double lat) || !double.TryParse(record.Longitude, out double lon))
                        {
                            var error = $"Invalid coordinates for {record.ICRISAT_accession_identifier}: Lat={record.Latitude}, Lon={record.Longitude}";
                            _logger.LogWarning(error);
                            errors.Add(error);
                            failedRecords++;
                            continue;
                        }

                        var apiKey = _configuration["OpenWeatherApi:ApiKey"];
                        var startDate = DateTimeOffset.UtcNow.AddDays(-30);
                        var endDate = DateTimeOffset.UtcNow;

                        var temperatureSum = 0.0;
                        var humiditySum = 0.0;
                        var rainfallSum = 0.0;
                        var successfulDates = 0;

                        // Process each day in the date range
                        for (var date = startDate; date <= endDate; date = date.AddDays(1))
                        {
                            var url = $"https://api.openweathermap.org/data/2.5/forecast/daily?" +
                                     $"lat={lat}&lon={lon}&cnt=1&units=metric&appid={apiKey}";

                            var response = await _httpClient.GetAsync(url);
                            var content = await response.Content.ReadAsStringAsync();

                            if (!response.IsSuccessStatusCode)
                            {
                                var error = $"API error for {record.ICRISAT_accession_identifier} on {date:yyyy-MM-dd}: {response.StatusCode}, {content}";
                                _logger.LogError(error);
                                errors.Add(error);
                                continue;
                            }

                            try
                            {
                                var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(content);

                                if (weatherData?.List != null && weatherData.List.Any())
                                {
                                    foreach (var dailyData in weatherData.List)
                                    {
                                        // Calculate daily average temperature
                                        var avgTemp = (dailyData.Temp.Day + dailyData.Temp.Night + dailyData.Temp.Eve + dailyData.Temp.Morn) / 4.0;

                                        temperatureSum += avgTemp;
                                        humiditySum += dailyData.Humidity;
                                        rainfallSum += dailyData.Rain ?? 0;
                                        successfulDates++;

                                        Console.WriteLine($"  - Processed data for {date:yyyy-MM-dd}: Temp={avgTemp:F1}°C, Humidity={dailyData.Humidity}%, Rainfall={dailyData.Rain ?? 0}mm");
                                    }
                                }
                            }
                            catch (JsonException ex)
                            {
                                var error = $"Failed to parse weather data for {record.ICRISAT_accession_identifier} at {date:yyyy-MM-dd}: {ex.Message}";
                                _logger.LogError(ex, error);
                                errors.Add(error);
                            }

                            // Add delay to avoid rate limiting
                            await Task.Delay(1200);
                        }

                        if (successfulDates > 0)
                        {
                            var avgTemp = temperatureSum / successfulDates;
                            var avgHumidity = humiditySum / successfulDates;
                            var totalRainfall = rainfallSum; // Keep total rainfall, don't average it

                            // Update the characteristics table
                            var existingRecord = await _context.Characteristics
                                .FirstOrDefaultAsync(c => c.ICRISAT_accession_identifier == record.ICRISAT_accession_identifier);

                            if (existingRecord != null)
                            {
                                existingRecord.Temperature = avgTemp;
                                existingRecord.Humidity = avgHumidity;
                                existingRecord.Rainfall = totalRainfall;
                                existingRecord.Timestamp = DateTime.UtcNow;
                                await _context.SaveChangesAsync();

                                results.Add(new WeatherResponse
                                {
                                    ICRISAT_accession_identifier = record.ICRISAT_accession_identifier,
                                    Temperature = avgTemp,
                                    Humidity = avgHumidity,
                                    Rainfall = totalRainfall,
                                    Latitude = lat,
                                    Longitude = lon,
                                    DataPoints = successfulDates
                                });

                                Console.WriteLine($"✓ Successfully processed {record.ICRISAT_accession_identifier} with data from {successfulDates} days");
                                successfulRecords++;
                            }
                            else
                            {
                                var error = $"No matching record found in characteristics table for {record.ICRISAT_accession_identifier}";
                                _logger.LogWarning(error);
                                errors.Add(error);
                                failedRecords++;
                            }
                        }
                        else
                        {
                            var error = $"No valid weather data obtained for {record.ICRISAT_accession_identifier}";
                            _logger.LogWarning(error);
                            errors.Add(error);
                            failedRecords++;
                        }
                    }
                    catch (Exception ex)
                    {
                        var error = $"Error processing {record.ICRISAT_accession_identifier}: {ex.Message}";
                        _logger.LogError(ex, error);
                        errors.Add(error);
                        failedRecords++;
                    }
                }

                Console.WriteLine("\nProcessing Summary:");
                Console.WriteLine($"Total Records: {totalRecords}");
                Console.WriteLine($"Successfully Processed: {successfulRecords}");
                Console.WriteLine($"Failed: {failedRecords}");
                Console.WriteLine($"Errors: {errors.Count}");

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    SuccessfullyProcessed = successfulRecords,
                    Failed = failedRecords,
                    Results = results,
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing all records");
                return StatusCode(500, new { message = "An error occurred while processing records", error = ex.Message });
            }
        }

        [HttpPost("history")]
        [ProducesResponseType(typeof(WeatherResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetHistoricalWeather([FromBody] WeatherRequest request)
        {
            try
            {
                _logger.LogInformation($"Processing weather request for accession: {request.ICRISAT_accession_identifier}");

                if (string.IsNullOrEmpty(request.ICRISAT_accession_identifier))
                {
                    return BadRequest(new { message = "ICRISAT_accession_identifier is required" });
                }

                // Get coordinates from passport_data table
                var passportRecord = await _context.PassportData
                    .FirstOrDefaultAsync(p => p.ICRISAT_accession_identifier == request.ICRISAT_accession_identifier);

                if (passportRecord == null || passportRecord.Latitude == null || passportRecord.Longitude == null)
                {
                    _logger.LogWarning($"No valid coordinates found for accession: {request.ICRISAT_accession_identifier}");
                    return NotFound(new { message = "No valid coordinates found for the given accession identifier" });
                }

                // Convert string coordinates to double
                if (!double.TryParse(passportRecord.Latitude, out double lat) || !double.TryParse(passportRecord.Longitude, out double lon))
                {
                    var error = $"Invalid coordinates for {request.ICRISAT_accession_identifier}: Lat={passportRecord.Latitude}, Lon={passportRecord.Longitude}";
                    _logger.LogWarning(error);
                    return BadRequest(new { message = error });
                }

                var apiKey = _configuration["OpenWeatherApi:ApiKey"];
                var days = request.EndTime.HasValue ? 
                    (int)((request.EndTime.Value - request.StartTime) / 86400) + 1 : 1;

                var url = $"https://api.openweathermap.org/data/2.5/forecast/daily?" +
                         $"lat={lat}&lon={lon}&cnt={days}&units=metric&appid={apiKey}";

                var response = await _httpClient.GetAsync(url);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    var error = $"API error: {response.StatusCode}, {content}";
                    _logger.LogError(error);
                    return StatusCode((int)response.StatusCode, new { message = error });
                }

                var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(content);

                if (weatherData?.List == null || !weatherData.List.Any())
                {
                    _logger.LogWarning($"No weather data found for accession: {request.ICRISAT_accession_identifier}");
                    return NotFound(new { message = "No weather data found for the given coordinates and date range" });
                }

                // Calculate averages across all days
                var avgTemp = weatherData.List.Average(d => (d.Temp.Day + d.Temp.Night + d.Temp.Eve + d.Temp.Morn) / 4.0);
                var avgHumidity = weatherData.List.Average(d => d.Humidity);
                var totalRainfall = weatherData.List.Sum(d => d.Rain ?? 0);

                // Update the characteristics table
                var existingRecord = await _context.Characteristics
                    .FirstOrDefaultAsync(c => c.ICRISAT_accession_identifier == request.ICRISAT_accession_identifier);

                if (existingRecord != null)
                {
                    existingRecord.Temperature = avgTemp;
                    existingRecord.Humidity = avgHumidity;
                    existingRecord.Rainfall = totalRainfall;
                    existingRecord.Timestamp = DateTime.UtcNow;
                    await _context.SaveChangesAsync();
                }

                return Ok(new WeatherResponse
                {
                    ICRISAT_accession_identifier = request.ICRISAT_accession_identifier,
                    Temperature = avgTemp,
                    Humidity = avgHumidity,
                    Rainfall = totalRainfall,
                    Latitude = lat,
                    Longitude = lon,
                    DataPoints = weatherData.List.Count
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing the weather request");
                return StatusCode(500, new { message = "An error occurred while processing the request", error = ex.Message });
            }
        }
    }
}
