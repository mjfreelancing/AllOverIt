using Amazon.CDK.AWS.AppSync;
using System;
using System.Reflection;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    public interface IGraphqlTypeStore
    {
        // Used when determining types from properties or method return values (memberInfo may contain custom attributes)
        GraphqlType GetGraphqlType(string parentName, SystemType type, MemberInfo memberInfo, bool isRequired, bool isList, bool isRequiredList,
            Action<IIntermediateType> typeCreated);

        // Used when determining types from method arguments (parameterInfo may contain custom attributes)
        GraphqlType GetGraphqlType(SystemType type, ParameterInfo parameterInfo, bool isRequired, bool isList, bool isRequiredList,
            Action<IIntermediateType> typeCreated);
    }
}