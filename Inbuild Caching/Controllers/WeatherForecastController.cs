using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Caching.Memory;

namespace Inbuild_Caching.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        private readonly IMemoryCache _memory;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IMemoryCache memory)
        {
            _logger = logger;
            _memory = memory;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        //[ResponseCache(Duration = 5)]
        //[OutputCache(Duration = 60)]
        public IActionResult Get()
        {

            const string cacheKey = "weatherResult";

            if(!_memory.TryGetValue(cacheKey , out IEnumerable<WeatherForecast> forecast)){
                forecast = Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();

                var cacheOptions = new MemoryCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1),
                    SlidingExpiration = TimeSpan.FromSeconds(30) // Resets if accessed within 30 seconds (sliding expiration)
                };

                _memory.Set(cacheKey, forecast, cacheOptions);
                

            }

            var result2 = Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
           .ToArray();

            return Ok(new {forecast, result2 });
        }
    }
}
