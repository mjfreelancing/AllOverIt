using AllOverIt.Extensions;
using EnrichedEnumModelBinding.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;

namespace EnrichedEnumModelBinding.Controllers
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public WeatherReport Get([FromQuery] WeatherRequest request)
        {
            var rng = new Random();

            // default if null or 'ForecastPeriod.ThisWeek'
            var dayOffset = 0;
            var dayCount = 7;

            var period = request.Period ?? ForecastPeriod.Default;  // Default is 'ThisWeek'

            if (period == ForecastPeriod.Today)
            {
                dayCount = 1;
            }
            else if (period == ForecastPeriod.Tomorrow)
            {
                dayOffset = 1;
                dayCount = 1;
            }
            else if(period == ForecastPeriod.NextWeek)
            {
                dayOffset = 7;
            }

            return new WeatherReport
            {
                Title = period.Name,
                Forecast = Enumerable
                    .Range(dayOffset, dayCount)
                    .SelectAsReadOnlyCollection(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = rng.Next(-20, 55),
                        Summary = Summaries[rng.Next(Summaries.Length)]
                    })
            };
        }

        // Test this by sending a Postman request with a body like this:
        //     {
        //       "period": "Today"
        //     }
        [HttpPost]
        public ActionResult Post([FromBody] WeatherRequest request)
        {
            if (request.Period == null)
            {
                return NoContent();
            }

            var result = new
            {
                request.Period.Value,
                request.Period.Name
            };

            return Ok(result);
        }
    }
}
