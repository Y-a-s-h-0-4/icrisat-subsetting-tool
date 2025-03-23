using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IcrisatSubsetting.Data;
using IcrisatSubsetting.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;

namespace IcrisatSubsetting.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "7eb738b52600ae4e9d514a712822ae13"; // Replace with your OpenWeather API key

        public WeatherController(AppDbContext context, HttpClient httpClient)
        {
            _context = context;
            _httpClient = httpClient;
        }

        [HttpPost("update-weather-data")]
        public async Task<IActionResult> UpdateWeatherData()
        {
            var passportDataList = await _context.PassportData.ToListAsync();
            if (passportDataList == null || passportDataList.Count == 0)
            {
                return NotFound("No records found in PassportData table.");
            }

            var updatedRecords = new List<Characteristics>();

            foreach (var record in passportDataList)
            {
                if (record.Latitude == null || record.Longitude == null)
                {
                    continue; // Skip if latitude or longitude is null
                }

                string apiUrl = $"https://api.openweathermap.org/data/2.5/weather?lat={record.Latitude}&lon={record.Longitude}&appid={_apiKey}&units=metric";

                try
                {
                    HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    using var jsonDoc = JsonDocument.Parse(responseBody);
                    var root = jsonDoc.RootElement;

                    double temperature = root.GetProperty("main").GetProperty("temp").GetDouble();
                    double humidity = root.GetProperty("main").GetProperty("humidity").GetDouble();
                    double rainfall = root.GetProperty("rain").TryGetProperty("1h", out JsonElement rainElement) ? rainElement.GetDouble() : 0.0;

                    var characteristics = await _context.Characteristics
                        .FirstOrDefaultAsync(c => c.ICRISAT_accession_identifier == record.ICRISAT_accession_identifier);

                    if (characteristics != null)
                    {
                        characteristics.Temperature = temperature;
                        characteristics.Precipitation = humidity;
                        characteristics.Rainfall = rainfall;
                        updatedRecords.Add(characteristics);
                    }
                }
                catch (HttpRequestException ex)
                {
                    return StatusCode(500, $"Error fetching weather data for {record.ICRISAT_accession_identifier}: {ex.Message}");
                }
            }

            if (updatedRecords.Count > 0)
            {
                await _context.SaveChangesAsync();
                return Ok($"{updatedRecords.Count} records updated successfully.");
            }

            return NotFound("No matching records found to update.");
        }
    }
}
