using AllOverIt.Patterns.ValueObject;
using ValueObjectDemo.Extensions;

namespace ValueObjectDemo
{
    internal sealed class TemperatureValueObject : ValueObject<EnrichedTemperature, TemperatureValueObject>
    {
        public TemperatureUnits Units { get; }
        public double Temperature => Value.Temperature;

        public TemperatureValueObject(double celcius)
            : this(new EnrichedTemperature(TemperatureUnits.Celcius, celcius))
        {
        }

        public TemperatureValueObject(TemperatureUnits units, double celcius)
            : this(new EnrichedTemperature(units, celcius))
        {
        }

        public TemperatureValueObject(EnrichedTemperature value)
            : base(value)
        {
            Units = value.Units;
        }

        protected override bool ValidateValue(EnrichedTemperature value)
        {
            var kelvin = value.ConvertToKelvin();
            return kelvin >= 0.0d;
        }
    }
}
