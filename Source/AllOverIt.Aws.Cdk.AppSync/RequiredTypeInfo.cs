using AllOverIt.Aws.Cdk.AppSync.Extensions;
using System.Reflection;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    public sealed class RequiredTypeInfo
    {
        public SystemType Type { get; }
        public bool IsRequired { get; }
        public bool IsList { get; }
        public bool IsRequiredList { get; }

        public RequiredTypeInfo(MethodInfo methodInfo)
        {
            Type = methodInfo.ReturnType;
            IsRequired = methodInfo.IsGqlTypeRequired();
            IsList = Type.IsArray;
            IsRequiredList = IsList && methodInfo.IsGqlArrayRequired();
        }

        public RequiredTypeInfo(ParameterInfo parameterInfo)
        {
            Type = parameterInfo.ParameterType;
            IsRequired = parameterInfo.IsGqlTypeRequired();
            IsList = Type.IsArray;
            IsRequiredList = IsList && parameterInfo.IsGqlArrayRequired();
        }
    }
}