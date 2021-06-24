using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Aws.Cdk.AppSync.MappingTemplates
{
    public abstract class MappingTemplatesBase : IMappingTemplates
    {
        private readonly IDictionary<string, string> _functionRequestMappings = new Dictionary<string, string>();
        private readonly IDictionary<string, string> _functionResponseMappings = new Dictionary<string, string>();

        public abstract string DefaultRequestMapping { get; }
        public abstract string DefaultResponseMapping { get; }

        public void RegisterRequestMapping(string functionName, string mapping)
        {
            _functionRequestMappings.Add(functionName, mapping);
        }

        public void RegisterResponseMapping(string functionName, string mapping)
        {
            _functionResponseMappings.Add(functionName, mapping);
        }

        public string GetRequestMapping(string functionName)
        {
            if (functionName.IsNullOrEmpty())
            {
                return DefaultRequestMapping;
            }

            var mapping = _functionRequestMappings.GetValueOrDefault(functionName);

            return mapping ?? DefaultRequestMapping;
        }

        public string GetResponseMapping(string functionName)
        {
            if (functionName.IsNullOrEmpty())
            {
                return DefaultResponseMapping;
            }

            var mapping = _functionResponseMappings.GetValueOrDefault(functionName);

            return mapping ?? DefaultResponseMapping;
        }
    }
}