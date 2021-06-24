namespace AllOverIt.Aws.Cdk.AppSync.MappingTemplates
{
    public interface IMappingTemplates
    {
        string DefaultRequestMapping { get; }
        string DefaultResponseMapping { get; }

        void RegisterRequestMapping(string functionName, string mapping);
        void RegisterResponseMapping(string functionName, string mapping);

        // if the function mapping is not defined then the default mapping is returned
        string GetRequestMapping(string functionName);
        string GetResponseMapping(string functionName);
    }
}