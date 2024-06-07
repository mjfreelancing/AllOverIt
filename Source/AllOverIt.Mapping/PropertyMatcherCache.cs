using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Mapping.Exceptions;
using System.Collections.Concurrent;

namespace AllOverIt.Mapping
{
    internal sealed class PropertyMatcherCache
    {
        // Maps source/target types to the property matcher configuration
        private readonly ConcurrentDictionary<(Type, Type), ObjectPropertyMatcher> _matcherCache = new();

        internal ObjectPropertyMatcher CreateMapper(Type sourceType, Type targetType, PropertyMatcherOptions matcherOptions)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));
            _ = matcherOptions.WhenNotNull(nameof(matcherOptions));

            if (TryGetMapper(sourceType, targetType, out _))
            {
                throw new ObjectMapperException($"Mapping already exists between {sourceType.GetFriendlyName()} and {targetType.GetFriendlyName()}.");
            }

            return GetOrCreate(sourceType, targetType, matcherOptions);
        }

        internal ObjectPropertyMatcher GetOrCreateMapper(Type sourceType, Type targetType)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));

            return GetOrCreate(sourceType, targetType, PropertyMatcherOptions.None);
        }

        internal bool TryGetMapper(Type sourceType, Type targetType, out ObjectPropertyMatcher matcher)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));

            var matcherKey = (sourceType, targetType);

            return _matcherCache.TryGetValue(matcherKey, out matcher);
        }

        private ObjectPropertyMatcher GetOrCreate(Type sourceType, Type targetType, PropertyMatcherOptions matcherOptions)
        {
            var matcherKey = (sourceType, targetType);

            // Not too concerned about atomicity
            return _matcherCache.GetOrAdd(matcherKey, _ =>
            {
                return new ObjectPropertyMatcher(
                    sourceType,
                    targetType,
                    matcherOptions);
            });
        }
    }
}