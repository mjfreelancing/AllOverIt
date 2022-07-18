using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Collections;
using AllOverIt.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync.Extensions
{
    internal static class ParameterInfoExtensions
    {
        private static readonly IReadOnlyDictionary<SystemType, string> EmptyTypeNameOverrides = Dictionary.EmptyReadOnly<SystemType, string>();

        public static RequiredTypeInfo GetRequiredTypeInfo(this ParameterInfo parameterInfo)
        {
            return new RequiredTypeInfo(parameterInfo);
        }

        public static bool IsGqlTypeRequired(this ParameterInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaTypeRequiredAttribute>(true) != null;
        }

        public static bool IsGqlArrayRequired(this ParameterInfo propertyInfo)
        {
            return propertyInfo.GetCustomAttribute<SchemaArrayRequiredAttribute>(true) != null;
        }

        public static void AssertParameterTypeIsNotNullable(this ParameterInfo parameterInfo)
        {
            var methodInfo = parameterInfo.Member as MethodInfo;

            if (parameterInfo.ParameterType.IsNullableType())
            {
                var arguments = string.Join(", ", methodInfo!.GetParameters().Select(parameter => $"{parameter.ParameterType.GetFriendlyName()} {parameter.Name}"));

                throw new SchemaException($"The argument '{parameterInfo.Name}' on method '{methodInfo.DeclaringType!.FullName}.{methodInfo.Name}({arguments})' " +
                                          $"cannot be nullable. The presence of {nameof(SchemaTypeRequiredAttribute)} is used to declare a parameter is required, " +
                                           "and its absence makes it optional.");
            }
        }

        public static void AssertParameterSchemaType(this ParameterInfo parameterInfo, MethodInfo methodInfo)
        {
            var requiredTypeInfo = parameterInfo.GetRequiredTypeInfo();
            var parameterSchemaType = requiredTypeInfo.Type.GetGraphqlTypeDescriptor(EmptyTypeNameOverrides).SchemaType;

            if (parameterSchemaType != GraphqlSchemaType.Scalar &&
                parameterSchemaType != GraphqlSchemaType.AWSScalar &&
                parameterSchemaType != GraphqlSchemaType.Input &&
                parameterSchemaType != GraphqlSchemaType.Enum)
            {
                throw new InvalidOperationException($"The argument '({parameterInfo.ParameterType.Name} {parameterInfo.Name})' passed to " +
                                                    $"{methodInfo.DeclaringType!.FullName}.{methodInfo.Name} must be either a scalar, enum, or an INPUT type.");
            }
        }
    }
}