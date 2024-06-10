using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Mapping
{
    /// <summary>Provides object mapping configuration that can be applied to an <see cref="ObjectMapper"/>.</summary>
    public sealed class ObjectMapperConfiguration
    {
        internal readonly PropertyMatcherCache _propertyMatcherCache = new();
        internal readonly ObjectMapperTypeFactory _typeFactory = new();

        /// <summary>Provides global mapping options.</summary>
        public IObjectMapperOptions Options { get; }

        /// <summary>Constructor. Initialized with a default constructed <see cref="PropertyMatcherOptions"/>.</summary>
        public ObjectMapperConfiguration()
        {
            Options = new ObjectMapperOptions(_typeFactory);
        }

        /// <summary>Constructor.</summary>
        /// <param name="configure">Provides the ability to configure default options for all non-configured mapping operations.</param>
        public ObjectMapperConfiguration(Action<IObjectMapperOptions> configure)
            : this()
        {
            configure
                .WhenNotNull(nameof(configure))
                .Invoke(Options);
        }

        /// <summary>Allows source to target mapping configuration to be initialized in advance.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="configure">The action that allows the property matching options to be configured.</param>
        public void Configure<TSource, TTarget>(Action<TypedPropertyMatcherOptions<TSource, TTarget>> configure = default)
        {
            var sourceType = typeof(TSource);
            var targetType = typeof(TTarget);

            var matcherOptions = PropertyMatcherOptions.None;

            if (configure is not null)
            {
                var typedMatcherOptions = new TypedPropertyMatcherOptions<TSource, TTarget>(_typeFactory.Add);

                configure.Invoke(typedMatcherOptions);

                matcherOptions = typedMatcherOptions;
            }

            _ = _propertyMatcherCache.CreateMapper(sourceType, targetType, matcherOptions);
        }

        internal Func<object> GetOrAdd(Type type, Func<object> factory)
        {
            _ = type.WhenNotNull(nameof(type));
            _ = factory.WhenNotNull(nameof(factory));

            return _typeFactory.GetOrAdd(type, factory);
        }

        internal Func<object> GetTypeFactory<TType>()
        {
            return GetTypeFactory(typeof(TType));
        }

        internal Func<object> GetTypeFactory(Type type)
        {
            _ = type.WhenNotNull(nameof(type));

            return _typeFactory.GetOrLazilyAdd(type, type.GetFactory);
        }
    }
}