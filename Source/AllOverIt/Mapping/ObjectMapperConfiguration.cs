using AllOverIt.Assertion;
using System;

namespace AllOverIt.Mapping
{
    /// <summary>Provides object mapping configuration that can be applied to an <see cref="ObjectMapper"/>.</summary>
    public sealed class ObjectMapperConfiguration
    {
        internal PropertyMatcherCache PropertyMatchers { get; }
        internal ObjectMapperTypeFactory TypeFactory { get; }

        /// <summary>Constructor.</summary>
        /// <param name="defaultOptions">Specifies default options for all mapping operations. If not provided then a default
        /// constructed <see cref="PropertyMatcherOptions"/> is used.</param>
        public ObjectMapperConfiguration(PropertyMatcherOptions defaultOptions = default)
            : this(new PropertyMatcherCache(defaultOptions), new ObjectMapperTypeFactory())
        {
        }

        internal ObjectMapperConfiguration(PropertyMatcherCache propertyMapperCache, ObjectMapperTypeFactory mapperTypeFactory)
        {
            PropertyMatchers = propertyMapperCache.WhenNotNull(nameof(propertyMapperCache));
            TypeFactory = mapperTypeFactory.WhenNotNull(nameof(mapperTypeFactory));
        }

        /// <summary>Allows source to target mapping configuration to be initialized in advance.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="configure">The action that allows the property matching options to be configured.</param>
        public void Configure<TSource, TTarget>(Action<TypedPropertyMatcherOptions<TSource, TTarget>> configure = default)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var matcherOptions = GetConfiguredOptionsOrDefault(configure);

            _ = PropertyMatchers.CreateMapper(sourceType, targetType, matcherOptions);
        }

        private PropertyMatcherOptions GetConfiguredOptionsOrDefault<TSource, TTarget>(Action<TypedPropertyMatcherOptions<TSource, TTarget>> configure)
        {
            if (configure is null)
            {
                return PropertyMatchers.DefaultOptions;
            }

            var matcherOptions = new TypedPropertyMatcherOptions<TSource, TTarget>((sourceType, targetType, factory) =>
            {
                TypeFactory.Add(sourceType, targetType, factory);
            });

            configure?.Invoke(matcherOptions);

            return matcherOptions;
        }
    }
}