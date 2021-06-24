using Amazon.CDK.AWS.AppSync;
using System;
using System.Reflection;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    public interface IGraphqlTypeStore
    {
        // Used when determining a root level type, such as a query or mutation
        GraphqlType GetGraphqlType(SystemType type, bool isRequired, bool isList, bool isRequiredList, Action<IIntermediateType> typeCreated);

        // Used when determining types from properties or method return values (memberInfo may contain custom attributes)
        GraphqlType GetGraphqlType(SystemType type, MemberInfo memberInfo, bool isRequired, bool isList, bool isRequiredList, Action<IIntermediateType> typeCreated);

        // Used when determining types from method arguments (parameterInfo may contain custom attributes)
        GraphqlType GetGraphqlType(SystemType type, ParameterInfo parameterInfo, bool isRequired, bool isList, bool isRequiredList, Action<IIntermediateType> typeCreated);
    }
}