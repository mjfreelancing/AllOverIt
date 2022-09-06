using AllOverIt.Assertion;
using System;
using System.Collections.Generic;

namespace AllOverIt.Mapping
{
    internal sealed class ObjectMapperTypeFactory
    {
        // Factories for any given type not associated with configuration
        private readonly IDictionary<Type, Func<object>> _typeFactories = new Dictionary<Type, Func<object>>();

        // Source => Target factories provided via configuration
        private readonly IDictionary<(Type, Type), Func<IObjectMapper, object, object>> _sourceTargetFactories
            = new Dictionary<(Type, Type), Func<IObjectMapper, object, object>>();

        public void Add(Type type, Func<object> factory)
        {
            _ = type.WhenNotNull(nameof(type));
            _ = factory.WhenNotNull(nameof(factory));

            _typeFactories.Add(type, factory);
        }

        public bool TryGet(Type type, out Func<object> factory)
        {
            _ = type.WhenNotNull(nameof(type));

            return _typeFactories.TryGetValue(type, out factory);
        }

        public void Add(Type sourceType, Type targetType, Func<IObjectMapper, object, object> factory)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));
            _ = factory.WhenNotNull(nameof(factory));

            var factoryKey = (sourceType, targetType);

            _sourceTargetFactories.Add(factoryKey, factory);
        }

        public bool TryGet(Type sourceType, Type targetType, out Func<IObjectMapper, object, object> factory)
        {
            _ = sourceType.WhenNotNull(nameof(sourceType));
            _ = targetType.WhenNotNull(nameof(targetType));

            var factoryKey = (sourceType, targetType);

            return _sourceTargetFactories.TryGetValue(factoryKey, out factory);
        }
    }
}