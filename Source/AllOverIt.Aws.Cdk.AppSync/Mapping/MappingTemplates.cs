using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.Mapping
{
    public sealed class MappingTemplates
    {
        private readonly IDictionary<string, string> _functionRequestMappings = new Dictionary<string, string>();
        private readonly IDictionary<string, string> _functionResponseMappings = new Dictionary<string, string>();

        public void RegisterMappings(string mappingKey, string requestMapping, string responseMapping)
        {
            _functionRequestMappings.Add(mappingKey, requestMapping);
            _functionResponseMappings.Add(mappingKey, responseMapping);
        }

        public MappingTemplate GetRequestMapping(string mappingKey)
        {
            var mapping = _functionRequestMappings.GetValueOrDefault(mappingKey);

            if (mapping == null)
            {
                throw new KeyNotFoundException($"Request mapping not found for the key '{mappingKey}'");
            }

            return MappingTemplate.FromString(mapping);
        }

        public MappingTemplate GetResponseMapping(string mappingKey)
        {
            var mapping = _functionResponseMappings.GetValueOrDefault(mappingKey); if (mapping == null)
            {
                throw new KeyNotFoundException($"Response mapping not found for the key '{mappingKey}'");
            }

            return MappingTemplate.FromString(mapping);
        }
    }
}