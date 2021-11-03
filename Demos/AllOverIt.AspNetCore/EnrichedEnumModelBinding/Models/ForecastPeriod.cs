using AllOverIt.Patterns.Enumeration;
using System.Runtime.CompilerServices;

namespace EnrichedEnumModelBinding.Models
{
    // Can apply a JsonConverter like this, or as per this demo within Startup.cs
    //[JsonConverter(typeof(ForecastPeriodConverter))]
    public sealed class ForecastPeriod : EnrichedEnum<ForecastPeriod>
    {
        public static readonly ForecastPeriod Today = new(1);
        public static readonly ForecastPeriod Tomorrow = new(2);
        public static readonly ForecastPeriod ThisWeek = new(3);
        public static readonly ForecastPeriod NextWeek = new(4);

        public static readonly ForecastPeriod Default = ThisWeek;

        private ForecastPeriod(int value, [CallerMemberName] string name = null)
            : base(value, name)
        {
        }
    }
}