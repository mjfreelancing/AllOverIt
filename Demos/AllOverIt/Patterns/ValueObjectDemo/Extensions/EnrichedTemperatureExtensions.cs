namespace ValueObjectDemo.Extensions
{
    internal static class EnrichedTemperatureExtensions
    {
        public static double ConvertToKelvin(this EnrichedTemperature enrichedTemperature)
        {
            return enrichedTemperature.Units switch
            {
                TemperatureUnits.Kelvin => enrichedTemperature.Temperature,
                TemperatureUnits.Celcius => enrichedTemperature.Temperature + 273.15,
                TemperatureUnits.Farenheit => (enrichedTemperature.Temperature - 32) * 5 / 9 + 273.15,
                _ => throw new ArgumentOutOfRangeException($"Unknown conversion from {enrichedTemperature.Units} to Kelvin")
            };
        }
    }
}
