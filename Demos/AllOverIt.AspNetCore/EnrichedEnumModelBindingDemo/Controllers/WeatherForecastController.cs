using AllOverIt.Extensions;
using AllOverIt.Validation;
using EnrichedEnumModelBindingDemo.Enums;
using EnrichedEnumModelBindingDemo.Models;
using EnrichedEnumModelBindingDemo.Requests;
using Microsoft.AspNetCore.Mvc;

namespace EnrichedEnumModelBindingDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly Random _random = new();

        private static readonly string[] Summaries =
        [
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        ];

        // Sample requests: http://localhost:5000/weatherforecast?period=1
        //                : http://localhost:5000/weatherforecast?period=nextweek
        [HttpGet]
        public WeatherReport Get([FromQuery] WeatherRequest request)
        {
            return GetWeatherReport(request.Period!);
        }

        // Tests model binding on query string of ForecastPeriodArray which is a ValueArray<ForecastPeriod>, where ForecastPeriod is an EnrichedEnum
        // Sample requests: http://localhost:5000/weatherforecast/multi?periods=today,tomorrow,nextweek
        [HttpGet("multi")]
        public WeatherReport[] GetMulti([FromQuery] WeatherRequestMulti request, [FromServices] ILifetimeValidationInvoker validationInvoker)
        {
            validationInvoker.AssertValidation(request);

            return request.Periods!.Values!.SelectToArray(GetWeatherReport) ?? [];
        }

        // Test this by sending a Postman request with a body like this:
        //     {
        //       "period": "Today"
        //     }
        [HttpPost]
        public ActionResult Post([FromBody] WeatherRequest request)
        {
            if (request.Period is null)
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

        private WeatherReport GetWeatherReport(ForecastPeriod period)
        {
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
                    .SelectToArray(index => new WeatherForecast
                    {
                        Date = DateTime.Now.AddDays(index),
                        TemperatureC = _random.Next(-20, 55),
                        Summary = Summaries[_random.Next(Summaries.Length)]
                    })
            };
        }
    }
}
