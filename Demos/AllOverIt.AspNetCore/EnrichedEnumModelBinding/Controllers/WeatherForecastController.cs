using AllOverIt.Extensions;
using EnrichedEnumModelBinding.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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

        // Sample requests: http://localhost:12365/weatherforecast?period=1
        //                : http://localhost:12365/weatherforecast?period=nextweek
        [HttpGet]
        public WeatherReport Get([FromQuery] WeatherRequest request)
        {
            return GetWeatherReport(request.Period);
        }

        // Tests model binding on query string of ForecastPeriodArray which is a ValueArray<ForecastPeriod>, where ForecastPeriod is an EnrichedEnum
        // Sample requests: http://localhost:12365/weatherforecast/multi?periods=today,tomorrow,nextweek
        [HttpGet("multi")]
        public IReadOnlyCollection<WeatherReport> GetMulti([FromQuery] WeatherRequestMulti request)
        {
            var periodArray = request.Periods ?? new ForecastPeriodArray();
            var periods = periodArray.Values ?? new List<ForecastPeriod>();

            return periods.SelectAsReadOnlyCollection(GetWeatherReport);
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

        private static WeatherReport GetWeatherReport(ForecastPeriod period)
        {
            var rng = new Random();

            // default if null or 'ForecastPeriod.ThisWeek'
            var dayOffset = 0;
            var dayCount = 7;

            period ??= ForecastPeriod.Default;          // Default is 'ThisWeek'

            if (period == ForecastPeriod.Today)
            {
                dayCount = 1;
            }
            else if (period == ForecastPeriod.Tomorrow)
            {
                dayOffset = 1;
                dayCount = 1;
            }
            else if (period == ForecastPeriod.NextWeek)
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
    }
}
