using AllOverIt.Patterns.ValueObject;
using ValueObjectDemo.Extensions;

namespace ValueObjectDemo
{
    internal sealed class TemperatureValueObject : ValueObject<EnrichedTemperature, TemperatureValueObject>
    {
        public TemperatureUnits Units { get; }
        public double Temperature => Value!.Temperature;

        public TemperatureValueObject(double celsius)
            : this(new EnrichedTemperature(TemperatureUnits.Celcius, celsius))
        {
        }

        public TemperatureValueObject(TemperatureUnits units, double celsius)
            : this(new EnrichedTemperature(units, celsius))
        {
        }

        public TemperatureValueObject(EnrichedTemperature value)
            : base(value)
        {
            Units = value.Units;
        }

        protected override bool ValidateValue(EnrichedTemperature? value)
        {
            if (value is null)
            {
                return false;
            }

            var kelvin = value.ConvertToKelvin();

            return kelvin >= 0.0d;
        }
    }
}
