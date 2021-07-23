using System;
using System.Reflection;

namespace AllOverIt.Aws.Cdk.AppSync.Factories
{
    public interface IResolverFactory
    {
        // Satisfies PropertyInfo and MethodInfo
        void ConstructResolverIfRequired(string parentName, Type type, MemberInfo propertyInfo);
    }
}