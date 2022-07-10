using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    /// <summary>A registry of AppSync datasource request and response mappings.</summary>
    public sealed class MappingTemplates
    {
        private readonly IDictionary<string, string> _functionRequestMappings = new Dictionary<string, string>();
        private readonly IDictionary<string, string> _functionResponseMappings = new Dictionary<string, string>();

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

            if (mapping == null)
            {
                throw new KeyNotFoundException($"Request mapping not found for the key '{mappingKey}'.");
            }

            return MappingTemplate.FromString(mapping);
        }

        /// <summary>Gets the datasource response mapping for the specified mapping key.</summary>
        /// <param name="mappingKey">The key associated with the response mapping.</param>
        /// <returns>The response mapping as an AppSync MappingTemplate.</returns>
        public MappingTemplate GetResponseMapping(string mappingKey)
        {
            var mapping = _functionResponseMappings.GetValueOrDefault(mappingKey); if (mapping == null)
            {
                throw new KeyNotFoundException($"Response mapping not found for the key '{mappingKey}'.");
            }

            return MappingTemplate.FromString(mapping);
        }
    }
}