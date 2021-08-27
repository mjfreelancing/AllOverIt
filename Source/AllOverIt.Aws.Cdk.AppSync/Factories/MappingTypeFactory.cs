using AllOverIt.Aws.Cdk.AppSync.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    public interface IMappingTypeFactory
    {
        IRequestResponseMapping GetRequestResponseMapping(SystemType mappingType);
    }

    public sealed class MappingTypeFactory : IMappingTypeFactory
    {
        private readonly IDictionary<SystemType, Func<IRequestResponseMapping>> _exactMappingRegistry = new Dictionary<SystemType, Func<IRequestResponseMapping>>();
        private readonly IDictionary<SystemType, Func<SystemType, IRequestResponseMapping>> _inheritedMappingRegistry = new Dictionary<SystemType, Func<SystemType, IRequestResponseMapping>>();

        // registers an exact mapping type (declared on a datasource attribute)
        public MappingTypeFactory Register<TType>(Func<IRequestResponseMapping> creator)
        {
            return Register(typeof(TType), creator);
        }

        // registers an exact mapping type (declared on a datasource attribute)
        public MappingTypeFactory Register(SystemType mappingType, Func<IRequestResponseMapping> creator)
        {
            _exactMappingRegistry.Add(mappingType, creator);
            return this;
        }

        // registers a func that will be called for any type that inherits TBaseType
        public MappingTypeFactory Register<TBaseType>(Func<SystemType, IRequestResponseMapping> creator)
        {
            return Register(typeof(TBaseType), creator);
        }

        // registers a func that will be called for any type that inherits TBaseType
        public MappingTypeFactory Register(Type baseMappingType, Func<SystemType, IRequestResponseMapping> creator)
        {
            _inheritedMappingRegistry.Add(baseMappingType, creator);
            return this;
        }

        IRequestResponseMapping IMappingTypeFactory.GetRequestResponseMapping(SystemType mappingType)
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
