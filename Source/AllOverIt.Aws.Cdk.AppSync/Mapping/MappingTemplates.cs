using AllOverIt.Assertion;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    /// <summary>A registry of AppSync datasource request and response mappings.</summary>
    public sealed class MappingTemplates
    {
        private readonly Dictionary<string, string> _functionRequestMappings = [];
        private readonly Dictionary<string, string> _functionResponseMappings = [];

        /// <summary>Registers a request and response mapping against a specified key.</summary>
        /// <param name="mappingKey">The key to register the mappings against.</param>
        /// <param name="requestMapping">The datasource request mapping as a string.</param>
        /// <param name="responseMapping">The datasource response mapping as a string.</param>
        public void RegisterMappings(string mappingKey, string requestMapping, string responseMapping)
        {
            _functionRequestMappings.Add(mappingKey, requestMapping);
            _functionResponseMappings.Add(mappingKey, responseMapping);
        }

        /// <summary>Gets the datasource request mapping for the specified mapping key.</summary>
        /// <param name="mappingKey">The key associated with the request mapping.</param>
        /// <returns>The request mapping as an AppSync MappingTemplate.</returns>
        public MappingTemplate GetRequestMapping(string mappingKey)
        {
            var mapping = _functionRequestMappings.GetValueOrDefault(mappingKey);

            Throw<KeyNotFoundException>.WhenNull(mapping, $"Request mapping not found for the key '{mappingKey}'.");

            return MappingTemplate.FromString(mapping);
        }

        /// <summary>Gets the datasource response mapping for the specified mapping key.</summary>
        /// <param name="mappingKey">The key associated with the response mapping.</param>
        /// <returns>The response mapping as an AppSync MappingTemplate.</returns>
        public MappingTemplate GetResponseMapping(string mappingKey)
        {
            var mapping = _functionResponseMappings.GetValueOrDefault(mappingKey);

            Throw<KeyNotFoundException>.WhenNull(mapping, $"Response mapping not found for the key '{mappingKey}'.");

            return MappingTemplate.FromString(mapping);
        }
    }
}