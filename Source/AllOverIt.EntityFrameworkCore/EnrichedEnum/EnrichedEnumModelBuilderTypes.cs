using AllOverIt.EntityFrameworkCore.ValueConverters;
using System;

namespace AllOverIt.EntityFrameworkCore.EnrichedEnum
{
    internal static class EnrichedEnumModelBuilderTypes
    {
        internal static readonly Type AsNameConverter = typeof(EnrichedEnumNameConverter<>);
        internal static readonly Type AsValueConverter = typeof(EnrichedEnumValueConverter<>);
    }
}