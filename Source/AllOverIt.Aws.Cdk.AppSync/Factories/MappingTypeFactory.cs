using AllOverIt.Aws.Cdk.AppSync.Mapping;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    /// <summary>A factory that creates an <see cref="IRequestResponseMapping"/> instance based on a registered type.</summary>
    public sealed class MappingTypeFactory
    {
        private readonly Dictionary<SystemType, Func<IRequestResponseMapping>> _exactMappingRegistry = [];
        private readonly Dictionary<SystemType, Func<SystemType, IRequestResponseMapping>> _inheritedMappingRegistry = [];

        /// <summary>Registers an exact mapping type (declared on a concrete <see cref="Attributes.DataSources.DataSourceAttribute"/> attribute)
        /// against a factory method to create an <see cref="IRequestResponseMapping"/> instance, which would typically be a <typeparamref name="TType"/>.</summary>
        /// <typeparam name="TType">The mapping type being registered.</typeparam>
        /// <param name="creator">The factory method to create the required <see cref="IRequestResponseMapping"/> instance.</param>
        /// <returns>Returns the <see cref="MappingTypeFactory"/> to allow for a fluent syntax.</returns>
        public MappingTypeFactory Register<TType>(Func<IRequestResponseMapping> creator) where TType : IRequestResponseMapping
        {
            return Register(typeof(TType), creator);
        }

        /// <summary>Registers an exact mapping type (declared on a concrete <see cref="Attributes.DataSources.DataSourceAttribute"/> attribute)
        /// against a factory method to create an <see cref="IRequestResponseMapping"/> instance.</summary>
        /// <param name="mappingType">The mapping type registered against the factory method.</param>
        /// <param name="creator">The factory method to create the required <see cref="IRequestResponseMapping"/> instance.</param>
        /// <returns>Returns the <see cref="MappingTypeFactory"/> to allow for a fluent syntax.</returns>
        public MappingTypeFactory Register(SystemType mappingType, Func<IRequestResponseMapping> creator)
        {
            if (!typeof(IRequestResponseMapping).IsAssignableFrom(mappingType))
            {
                throw new InvalidOperationException($"The mapping type '{mappingType.Name}' must inherit '{nameof(IRequestResponseMapping)}'.");
            }

            _exactMappingRegistry.Add(mappingType, creator);
            return this;
        }

        /// <summary>Registers a base mapping type (declared on a concrete <see cref="Attributes.DataSources.DataSourceAttribute"/> attribute)
        /// against a factory method to create an <see cref="IRequestResponseMapping"/> instance.</summary>
        /// <typeparam name="TBaseType">The base mapping type registered against the factory method.</typeparam>
        /// <param name="creator">The factory method to create the required <see cref="IRequestResponseMapping"/> instance.</param>
        /// <returns>Returns the <see cref="MappingTypeFactory"/> to allow for a fluent syntax.</returns>
        public MappingTypeFactory Register<TBaseType>(Func<SystemType, IRequestResponseMapping> creator) where TBaseType : IRequestResponseMapping
        {
            return Register(typeof(TBaseType), creator);
        }

        /// <summary>Registers a base mapping type (declared on a concrete <see cref="Attributes.DataSources.DataSourceAttribute"/> attribute)
        /// against a factory method to create an <see cref="IRequestResponseMapping"/> instance.</summary>
        /// <param name="baseMappingType">The base mapping type registered against the factory method.</param>
        /// <param name="creator">The factory method to create the required <see cref="IRequestResponseMapping"/> instance.</param>
        /// <returns>Returns the <see cref="MappingTypeFactory"/> to allow for a fluent syntax.</returns>
        public MappingTypeFactory Register(SystemType baseMappingType, Func<SystemType, IRequestResponseMapping> creator)
        {
            if (!typeof(IRequestResponseMapping).IsAssignableFrom(baseMappingType))
            {
                throw new InvalidOperationException($"The mapping type '{baseMappingType.Name}' must inherit '{nameof(IRequestResponseMapping)}'.");
            }

            _inheritedMappingRegistry.Add(baseMappingType, creator);
            return this;
        }

        internal IRequestResponseMapping GetRequestResponseMapping(SystemType mappingType)
        {
            // look for an exact match first
            if (_exactMappingRegistry.TryGetValue(mappingType, out var creator))
            {
                return creator.Invoke();
            }

            // next look for inherited types
            var baseType = _inheritedMappingRegistry.Keys.SingleOrDefault(item => item.IsAssignableFrom(mappingType));

            if (baseType != null)
            {
                return _inheritedMappingRegistry[baseType].Invoke(mappingType);
            }

            // assume there is default constructor
            return (IRequestResponseMapping) Activator.CreateInstance(mappingType);
        }
    }
}
