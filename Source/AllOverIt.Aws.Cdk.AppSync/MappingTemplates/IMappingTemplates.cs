namespace AllOverIt.Aws.Cdk.AppSync.MappingTemplates
{
    public interface IMappingTemplates
    {
        void RegisterMappings(string mappingKey, string requestMapping, string responseMapping);
        string GetRequestMapping(string mappingKey);
        string GetResponseMapping(string mappingKey);
    }
}