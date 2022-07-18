using AllOverIt.Patterns.Enumeration;
using System.Runtime.CompilerServices;

namespace GraphqlSchema.Schema.Types
{
    // Demonstrating that EnrichedEnum can be used to build GQL enum types.
    internal sealed class DateFormat : EnrichedEnum<DateFormat>
    {
        public static readonly DateFormat Universal = new(0);
        public static readonly DateFormat Local = new(1);

        private DateFormat(int value, [CallerMemberName] string name = null)
            : base(value, name)
        {
        }
    }
}