using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using WeatherAPI.Models;
using System.Threading.Tasks;
using System.Linq;

namespace WeatherAPI.Controllers
{
    [Route("api/weather")]
    [ApiController]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey = "7eb738b52600ae4e9d514a712822ae13"; // Replace with your OpenWeather API Key

        public WeatherController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
        }

        [HttpPost("history")]
        public async Task<IActionResult> GetHistoricalWeather([FromBody] WeatherRequest request)
        {
            if (request == null || request.Latitude == 0 || request.Longitude == 0 || request.StartTime == 0 || request.EndTime == 0)
            {
                return BadRequest("Invalid input. Provide latitude, longitude, startTime, and endTime.");
            }

            string url = $"https://history.openweathermap.org/data/2.5/history/city?lat={request.Latitude}&lon={request.Longitude}&type=hour&start={request.StartTime}&end={request.EndTime}&appid={_apiKey}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);
            string jsonResponse = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"API Response: {jsonResponse}");

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode((int)response.StatusCode, $"Error: {jsonResponse}");
            }

            var weatherData = JsonSerializer.Deserialize<WeatherApiResponse>(jsonResponse);

            if (weatherData == null || weatherData.List == null || weatherData.List.Count == 0)
            {
                return NotFound("No weather data found for the given time range.");
            }

            double avgTemp = weatherData.List.Average(w => w.Main.Temperature);
            double avgHumidity = weatherData.List.Average(w => w.Main.Humidity);
            double avgPressure = weatherData.List.Average(w => w.Main.Pressure);

            var result = new
            {
                AverageTemperature = avgTemp,
                AverageHumidity = avgHumidity,
                AveragePressure = avgPressure
            };

            return Ok(result);
        }
    }
}
