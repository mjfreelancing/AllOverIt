using System;
using ValueObjectDemo.Extensions;

namespace ValueObjectDemo
{
    internal sealed class EnrichedTemperature : IEquatable<EnrichedTemperature>, IComparable<EnrichedTemperature>
    {
        public TemperatureUnits Units { get; set; }
        public double Temperature { get; set; }

        public EnrichedTemperature()
        {
        }

        public EnrichedTemperature(TemperatureUnits units, double temperature)
        {
            Units = units;
            Temperature = temperature;
        }

        public bool Equals(EnrichedTemperature other)
        {
            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return other is not null && this.ConvertToKelvin().Equals(other.ConvertToKelvin());
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EnrichedTemperature);
        }

        public int CompareTo(EnrichedTemperature other)
        {
            return this.ConvertToKelvin().CompareTo(other.ConvertToKelvin());
        }

        public override int GetHashCode()
        {
            return this.ConvertToKelvin().GetHashCode();
        }
    }
}
