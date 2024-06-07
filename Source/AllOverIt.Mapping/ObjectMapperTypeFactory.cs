using AllOverIt.Assertion;
using System.Collections.Concurrent;

namespace AllOverIt.Mapping
{
    internal sealed class ObjectMapperTypeFactory
    {
        // Factories for any given type not associated with configuration
        private readonly ConcurrentDictionary<Type, Func<object>> _typeFactories = new();

        // Source => Target factories provided via configuration
        // Not a ConcurrentDictionary because this is only populated via explicit configuration
        private readonly Dictionary<(Type, Type), Func<IObjectMapper, object, object>> _sourceTargetFactories = [];

        internal void Add(Type sourceType, Type targetType, Func<IObjectMapper, object, object> factory)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));
            _ = factory.WhenNotNull(nameof(factory));

            var factoryKey = (sourceType, targetType);

            _sourceTargetFactories.Add(factoryKey, factory);
        }

        internal bool TryGet(Type sourceType, Type targetType, out Func<IObjectMapper, object, object> factory)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));

            var factoryKey = (sourceType, targetType);

            return _sourceTargetFactories.TryGetValue(factoryKey, out factory);
        }

        internal Func<object> GetOrAdd(Type type, Func<object> factory)
        {
            _ = type.WhenNotNull(nameof(type));
            _ = factory.WhenNotNull(nameof(factory));

            return _typeFactories.GetOrAdd(type, factory);
        }

        // Used internally when an explicit factory has not been registered
        // via ObjectMapperConfiguration (by calling GetOrAdd() in this class).
        internal Func<object> GetOrLazilyAdd(Type type, Func<Func<object>> factoryResolver)
        {
            _ = type.WhenNotNull(nameof(type));
            _ = factoryResolver.WhenNotNull(nameof(factoryResolver));

            if (!_typeFactories.TryGetValue(type, out var typeFactory))
            {
                typeFactory = factoryResolver.Invoke();

                _typeFactories.GetOrAdd(type, typeFactory);
            }

            return typeFactory;
        }
    }
}