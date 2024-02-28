using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Resolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    /// <summary>A factory that creates a resolver runtime instance based on a registered type. The instance typically
    /// implements <see cref="IVtlRuntime"/> or <see cref="IJsRuntime"/></summary>
    public sealed class ResolverFactory
    {
        private readonly Dictionary<SystemType, Func<IResolverRuntime>> _exactResolverRegistry = [];
        private readonly Dictionary<SystemType, Func<SystemType, IResolverRuntime>> _inheritedResolverRegistry = [];

        /// <summary>Registers an exact resolver type (declared on a concrete <see cref="Attributes.DataSources.DataSourceAttribute"/> attribute)
        /// against a factory method to create an <see cref="IVtlRuntime"/> instance, which would typically be a <typeparamref name="TType"/>.</summary>
        /// <typeparam name="TType">The resolver type being registered.</typeparam>
        /// <param name="creator">The factory method to create the required <see cref="IVtlRuntime"/> instance.</param>
        /// <returns>Returns the <see cref="ResolverFactory"/> to allow for a fluent syntax.</returns>
        public ResolverFactory Register<TType>(Func<IResolverRuntime> creator) where TType : IResolverRuntime
        {
            return Register(typeof(TType), creator);
        }

        /// <summary>Registers an exact resolver type (declared on a concrete <see cref="Attributes.DataSources.DataSourceAttribute"/> attribute)
        /// against a factory method to create an <see cref="IVtlRuntime"/> instance.</summary>
        /// <param name="resolverType">The resolver type registered against the factory method.</param>
        /// <param name="creator">The factory method to create the required <see cref="IVtlRuntime"/> instance.</param>
        /// <returns>Returns the <see cref="ResolverFactory"/> to allow for a fluent syntax.</returns>
        public ResolverFactory Register(SystemType resolverType, Func<IResolverRuntime> creator)
        {
            if (!typeof(IResolverRuntime).IsAssignableFrom(resolverType))
            {
                throw new SchemaException($"The resolver type '{resolverType.Name}' must inherit '{nameof(IResolverRuntime)}'.");
            }

            _exactResolverRegistry.Add(resolverType, creator);
            return this;
        }

        /// <summary>Registers a base resolver runtime type against a factory method. The resolver would typically implement
        /// <see cref="IVtlRuntime"/> or <see cref="IJsRuntime"/>.</summary>
        /// <typeparam name="TBaseType">The base resolver type registered against the factory method.</typeparam>
        /// <param name="creator">The factory method to create the required resolver instance.</param>
        /// <returns>Returns the <see cref="ResolverFactory"/> to allow for a fluent syntax.</returns>
        public ResolverFactory Register<TBaseType>(Func<SystemType, IResolverRuntime> creator) where TBaseType : IResolverRuntime
        {
            return Register(typeof(TBaseType), creator);
        }

        /// <summary>Registers a base resolver runtime type against a factory method. The resolver would typically implement
        /// <see cref="IVtlRuntime"/> or <see cref="IJsRuntime"/>.</summary>
        /// <param name="baseResolverType">The base resolver type registered against the factory method.</param>
        /// <param name="creator">The factory method to create the required resolver instance.</param>
        /// <returns>Returns the <see cref="ResolverFactory"/> to allow for a fluent syntax.</returns>
        public ResolverFactory Register(SystemType baseResolverType, Func<SystemType, IResolverRuntime> creator)
        {
            if (!typeof(IResolverRuntime).IsAssignableFrom(baseResolverType))
            {
                throw new SchemaException($"The resolver type '{baseResolverType.Name}' must inherit '{nameof(IResolverRuntime)}'.");
            }

            _inheritedResolverRegistry.Add(baseResolverType, creator);

            return this;
        }

        internal IResolverRuntime GetResolverRuntime(SystemType resolverType)
        {
            // Look for an exact match first.
            if (_exactResolverRegistry.TryGetValue(resolverType, out var creator))
            {
                return creator.Invoke();
            }

            // Next look for inherited types.
            var baseType = _inheritedResolverRegistry.Keys.SingleOrDefault(item => item.IsAssignableFrom(resolverType));

            if (baseType != null)
            {
                return _inheritedResolverRegistry[baseType].Invoke(resolverType);
            }

            // Assume there is default constructor.
            return (IResolverRuntime) Activator.CreateInstance(resolverType);
        }
    }
}
