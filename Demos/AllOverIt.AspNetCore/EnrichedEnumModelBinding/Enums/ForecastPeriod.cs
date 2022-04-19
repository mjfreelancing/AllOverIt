using System.ComponentModel;
using System.Runtime.CompilerServices;
using AllOverIt.Converters;
using AllOverIt.Patterns.Enumeration;

namespace EnrichedEnumModelBinding.Enums
{
    // Can apply a JsonConverter like this, or as per this demo within Startup.cs
    //[JsonConverter(typeof(ForecastPeriodConverter))]

    // The type converter is for the /multi endpoint (query string with an array of periods)
    [TypeConverter(typeof(EnrichedEnumTypeConverter<ForecastPeriod>))]
    public sealed class ForecastPeriod : EnrichedEnum<ForecastPeriod>
    {
        public static readonly ForecastPeriod Today = new(0);
        public static readonly ForecastPeriod Tomorrow = new(1);
        public static readonly ForecastPeriod ThisWeek = new(2);
        public static readonly ForecastPeriod NextWeek = new(3);

        public static readonly ForecastPeriod Default = ThisWeek;

        private ForecastPeriod(int value, [CallerMemberName] string name = null)
            : base(value, name)
        {
        }
    }
}