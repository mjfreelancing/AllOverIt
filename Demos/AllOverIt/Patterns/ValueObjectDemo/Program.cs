using AllOverIt.Patterns.ValueObject.Exceptions;
using ValueObjectDemo.Extensions;

namespace ValueObjectDemo
{
    internal class Program
    {
        static void Main()
        {
            // A temperature cannot be less than 0 Kelvin. The `TemperatureValueObject` uses a double to store the temperature
            // but it provides it's own domain-specific validation. To make the example a little more elaborate, the value object
            // is using an `EnrichedTemperature` type that stores the value and allows for conversion between different units.
            var temp1 = new TemperatureValueObject(TemperatureUnits.Kelvin, 0.0d);
            var temp2 = new TemperatureValueObject(20);                                 // defaults to Celcius
            var temp3 = new TemperatureValueObject(TemperatureUnits.Farenheit, -4);
            var temp4 = new TemperatureValueObject(-20);
            var temp5 = new TemperatureValueObject(-273.15d);

            try
            {
                _ = new TemperatureValueObject(-300d);                                       // will throw as the temperature is invalid
            }
            catch (ValueObjectValidationException exception)
            {
                var value = (EnrichedTemperature) (exception.AttemptedValue);
                throw new ArgumentException($"The temperature {value.Temperature:N2} {value.Units} is invalid ({value.ConvertToKelvin():N2} Kelvin)");
            }

            CompareTemperatures(temp1, temp2);
            CompareTemperatures(temp1, temp3);
            CompareTemperatures(temp1, temp4);
            CompareTemperatures(temp1, temp5);

            Console.WriteLine();

            CompareTemperatures(temp2, temp1);
            CompareTemperatures(temp2, temp3);
            CompareTemperatures(temp2, temp4);
            CompareTemperatures(temp2, temp5);

            Console.WriteLine();

            CompareTemperatures(temp3, temp1);
            CompareTemperatures(temp3, temp2);
            CompareTemperatures(temp3, temp4);
            CompareTemperatures(temp3, temp5);

            Console.WriteLine();

            CompareTemperatures(temp4, temp1);
            CompareTemperatures(temp4, temp2);
            CompareTemperatures(temp4, temp3);
            CompareTemperatures(temp4, temp5);

            Console.WriteLine();
            Console.WriteLine("All Over It.");
            Console.ReadKey();
        }

        private static void CompareTemperatures(TemperatureValueObject lhs, TemperatureValueObject rhs)
        {
            var same = lhs == rhs;
            var comparison = same ? "is the same as" : "is not the same as";

            Console.WriteLine($"{lhs.Temperature} {lhs.Units} {comparison} {rhs.Temperature} {rhs.Units}");
        }
    }
}
