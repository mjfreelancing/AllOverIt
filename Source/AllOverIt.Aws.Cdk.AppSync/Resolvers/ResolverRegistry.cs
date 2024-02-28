using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Resolvers;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Resolvers
{
    /// <summary>A registry of AppSync resolver runtime mappings.</summary>
    public sealed class ResolverRegistry
    {
        private sealed class RequestResponseMapping : IVtlRuntime
        {
            public string RequestMapping { get; init; }
            public string ResponseMapping { get; init; }
        }

        private sealed class CodeMapping : IJsRuntime
        {
            public string Code { get; init; }

            public FunctionRuntime FunctionRuntime { get; init; }
        }

        private readonly Dictionary<string, IResolverRuntime> _resolvers = [];

        /// <summary>Registers an <see cref="IResolverRuntime"/> instance with a specified key. This method is
        /// called internally when discovering attributes such as <see cref="UnitResolverAttribute"/>. It can also
        /// be called manually if the resolver type is not specified on the attribute because it requires
        /// special handling, such as explicit construction with runtime specified arguments</summary>
        /// <param name="resolverKey">The key identifying the resolver being registered. This key is a flattened
        /// dot-notation representation of the field the resolver is registered against, such as 'Query.Field1.Field2'.</param>
        /// <param name="resolver">The resolver instance to be registered.</param>
        public void RegisterResolver(string resolverKey, IResolverRuntime resolver)
        {
            _resolvers.Add(resolverKey, resolver);
        }

        /// <summary>Gets a resolver based on the specified key.</summary>
        /// <param name="resolverKey">The key identifying the registered resolver.</param>
        /// <returns>The resolver instance.</returns>
        public IResolverRuntime GetResolver(string resolverKey)
        {
            var resolver = _resolvers.GetValueOrDefault(resolverKey);

            Throw<SchemaException>.WhenNull(resolver, $"Resolver not found for the key '{resolverKey}'.");

            return resolver;
        }

        internal void SetResolverProps(string resolverKey, ExtendedResolverProps resolverProps)
        {
            var resolver = _resolvers.GetValueOrDefault(resolverKey);

            Throw<SchemaException>.WhenNull(resolver, $"Resolver not found for the key '{resolverKey}'.");

            if (resolver is IVtlRuntime requestResponseMapping)
            {
                resolverProps.RequestMappingTemplate = MappingTemplate.FromString(requestResponseMapping.RequestMapping);
                resolverProps.ResponseMappingTemplate = MappingTemplate.FromString(requestResponseMapping.ResponseMapping);
            }
            else if (resolver is IJsRuntime jsRuntime)
            {
                resolverProps.Runtime = jsRuntime.FunctionRuntime;
                resolverProps.Code = Code.FromInline(jsRuntime.Code);
            }
            else
            {
                throw new InvalidOperationException($"Unhandled resolver type '{resolver.GetType().GetFriendlyName()}'");
            }
        }
    }
}