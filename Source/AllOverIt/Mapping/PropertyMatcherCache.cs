﻿using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Mapping.Exceptions;
using System;
using System.Collections.Generic;

namespace AllOverIt.Mapping
{
    internal sealed class PropertyMatcherCache
    {
        // Maps source/target types to the property matcher configuration
        private readonly IDictionary<(Type, Type), ObjectPropertyMatcher> _matcherCache = new Dictionary<(Type, Type), ObjectPropertyMatcher>();

        public ObjectPropertyMatcher CreateMapper(Type sourceType, Type targetType, PropertyMatcherOptions matcherOptions)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));
            _ = matcherOptions.WhenNotNull(nameof(matcherOptions));

            var matcherKey = (sourceType, targetType);

            if (_matcherCache.TryGetValue(matcherKey, out _))
            {
                throw new ObjectMapperException($"Mapping already exists between {sourceType.GetFriendlyName()} and {targetType.GetFriendlyName()}.");
            }

            var propertyMatcher = new ObjectPropertyMatcher(
                sourceType,
                targetType,
                matcherOptions);

            _matcherCache.Add(matcherKey, propertyMatcher);

            return propertyMatcher;
        }

        public ObjectPropertyMatcher GetOrCreateMapper(Type sourceType, Type targetType)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));

            return TryGetMapper(sourceType, targetType, out var matcher)
                ? matcher
                : CreateMapper(sourceType, targetType, PropertyMatcherOptions.None);
        }

        internal bool TryGetMapper(Type sourceType, Type targetType, out ObjectPropertyMatcher matcher)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));

            var matcherKey = (sourceType, targetType);

            return _matcherCache.TryGetValue(matcherKey, out matcher);
        }
    }
}