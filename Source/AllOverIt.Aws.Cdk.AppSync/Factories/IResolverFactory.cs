using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    public interface IResolverFactory
    {
        // Satisfies PropertyInfo and MethodInfo
        void ConstructResolverIfRequired(string parentName, string schemaTypeName, MemberInfo propertyInfo);
    }
}