﻿using AllOverIt.Assertion;
using AllOverIt.Reflection;
using System.Reflection;

namespace AllOverIt.Mapping
{
    internal sealed class ObjectPropertyMatcher
    {
        internal sealed class PropertyMatchInfo
        {
            public PropertyInfo SourceInfo { get; }
            public PropertyInfo TargetInfo { get; }
            public Func<object, object?> SourceGetter { get; }
            public Action<object, object?> TargetSetter { get; }

            public PropertyMatchInfo(PropertyInfo sourceInfo, PropertyInfo targetInfo)
            {
                SourceInfo = sourceInfo;
                TargetInfo = targetInfo;

                SourceGetter = PropertyHelper.CreatePropertyGetter(SourceInfo);
                TargetSetter = PropertyHelper.CreatePropertySetter(TargetInfo);
            }
        }

        public PropertyMatcherOptions MatcherOptions { get; }
        public PropertyMatchInfo[] Matches { get; }

        internal ObjectPropertyMatcher(Type sourceType, Type targetType, PropertyMatcherOptions matcherOptions)
        {
            _ = sourceType.WhenNotNull();
            _ = targetType.WhenNotNull();
            MatcherOptions = matcherOptions.WhenNotNull();

            // Find properties that match between the source and target (or have an alias) and meet any filter criteria.
            var matches = ObjectMapperHelper.GetMappableProperties(sourceType, targetType, matcherOptions);

            var sourcePropertyInfo = ReflectionCache
                .GetPropertyInfo(sourceType, matcherOptions.Binding)!
                .ToDictionary(prop => prop.Name);

            var targetPropertyInfo = ReflectionCache
                .GetPropertyInfo(targetType, matcherOptions.Binding)!
                .ToDictionary(prop => prop.Name);

            var matchedProps = new List<PropertyMatchInfo>(matches.Length);

            foreach (var match in matches)
            {
                var sourcePropInfo = sourcePropertyInfo[match.Name];
                var targetName = ObjectMapperHelper.GetTargetAliasName(match.Name, matcherOptions);
                var targetPropInfo = targetPropertyInfo[targetName];

                var matchInfo = new PropertyMatchInfo(sourcePropInfo, targetPropInfo);
                matchedProps.Add(matchInfo);
            }

            Matches = [.. matchedProps];
        }
    }
}