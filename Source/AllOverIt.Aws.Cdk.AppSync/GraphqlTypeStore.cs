﻿using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Attributes.Types;
using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnumType = Amazon.CDK.AWS.AppSync.EnumType;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    internal sealed class GraphqlTypeStore
    {
        private readonly IList<SystemType> _typeUnderConstruction = new List<SystemType>();
        private readonly GraphqlApi _graphqlApi;
        private readonly MappingTemplates _mappingTemplates;
        private readonly MappingTypeFactory _mappingTypeFactory;
        private readonly DataSourceFactory _dataSourceFactory;

        private readonly IDictionary<string, Func<RequiredTypeInfo, GraphqlType>> _fieldTypes = new Dictionary<string, Func<RequiredTypeInfo, GraphqlType>>
        {
            {nameof(GraphqlTypeId), requiredTypeInfo => GraphqlType.Id(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypePhone), requiredTypeInfo => GraphqlType.AwsPhone(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypeEmail), requiredTypeInfo => GraphqlType.AwsEmail(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypeIpAddress), requiredTypeInfo => GraphqlType.AwsIpAddress(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypeJson), requiredTypeInfo => GraphqlType.AwsJson(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypeUrl), requiredTypeInfo => GraphqlType.AwsUrl(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypeTimestamp), requiredTypeInfo => GraphqlType.AwsTimestamp(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypeDate), requiredTypeInfo => GraphqlType.AwsDate(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypeTime), requiredTypeInfo => GraphqlType.AwsTime(CreateTypeOptions(requiredTypeInfo))},
            {nameof(AwsTypeDateTime), requiredTypeInfo => GraphqlType.AwsDateTime(CreateTypeOptions(requiredTypeInfo))},
            {nameof(Int32), requiredTypeInfo => GraphqlType.Int(CreateTypeOptions(requiredTypeInfo))},
            {nameof(Double), requiredTypeInfo => GraphqlType.Float(CreateTypeOptions(requiredTypeInfo))},
            {nameof(Single), requiredTypeInfo => GraphqlType.Float(CreateTypeOptions(requiredTypeInfo))},
            {nameof(Boolean), requiredTypeInfo => GraphqlType.Boolean(CreateTypeOptions(requiredTypeInfo))},
            {nameof(String), requiredTypeInfo => GraphqlType.String(CreateTypeOptions(requiredTypeInfo))}
        };

        public GraphqlTypeStore(GraphqlApi graphqlApi, MappingTemplates mappingTemplates, MappingTypeFactory mappingTypeFactory, DataSourceFactory dataSourceFactory)
        {
            _graphqlApi = graphqlApi.WhenNotNull(nameof(graphqlApi));
            _mappingTemplates = mappingTemplates.WhenNotNull(nameof(mappingTemplates));
            _mappingTypeFactory = mappingTypeFactory.WhenNotNull(nameof(mappingTypeFactory));
            _dataSourceFactory = dataSourceFactory.WhenNotNull(nameof(dataSourceFactory));
        }

        public GraphqlType GetGraphqlType(string fieldName, RequiredTypeInfo requiredTypeInfo, Action<IIntermediateType> typeCreated)
        {
            SchemaUtils.AssertNoProperties(requiredTypeInfo.Type);

            var typeDescriptor = requiredTypeInfo.Type.GetGraphqlTypeDescriptor();
            var typeName = typeDescriptor.Name;

            var fieldTypeCreator = GetTypeCreator(fieldName, requiredTypeInfo.Type, typeName, typeDescriptor, typeCreated);

            return fieldTypeCreator.Invoke(requiredTypeInfo);
        }

        private Func<RequiredTypeInfo, GraphqlType> GetTypeCreator(string parentName, SystemType type, string lookupTypeName,
            GraphqlSchemaTypeDescriptor typeDescriptor, Action<IIntermediateType> typeCreated)
        {
            if (!_fieldTypes.TryGetValue(lookupTypeName, out var fieldTypeCreator))
            {
                var elementType = type.GetElementTypeIfArray();

                var objectType = elementType!.IsEnum
                    ? CreateEnumType(elementType, typeDescriptor)
                    : CreateInterfaceType(parentName, elementType, typeDescriptor);

                // notify of type creation so it can, for example, be added to a schema
                typeCreated.Invoke(objectType);

                fieldTypeCreator = _fieldTypes[lookupTypeName];
            }

            return fieldTypeCreator;
        }

        private IIntermediateType CreateEnumType(SystemType type, GraphqlSchemaTypeDescriptor typeDescriptor)
        {
            var enumType = new EnumType(typeDescriptor.Name, new EnumTypeOptions
            {
                Definition = type.GetEnumNames().Select(item => item.ToUpperSnakeCase()).ToArray()
            });

            _fieldTypes.Add(
                typeDescriptor.Name,
                requiredTypeInfo => enumType.Attribute(CreateTypeOptions(requiredTypeInfo)));

            return enumType;
        }

        private IIntermediateType CreateInterfaceType(string parentName, SystemType type, GraphqlSchemaTypeDescriptor typeDescriptor)
        {
            SchemaUtils.AssertNoProperties(type);

            try
            {
                if (_typeUnderConstruction.Contains(type))
                {
                    var typeNames = string.Join(" -> ", _typeUnderConstruction.Select(item => item.Name).Concat(new[] { type.Name }));
                    throw new InvalidOperationException($"Unexpected re-entry while creating '{typeNames}'");
                }

                _typeUnderConstruction.Add(type);

                var classDefinition = new Dictionary<string, IField>();

                ParseInterfaceTypeMethods(parentName, classDefinition, type);

                var intermediateType = CreateIntermediateType(typeDescriptor, classDefinition);

                // Not currently validating auth modes on each field is also on its' return type because it's also possible to define
                // them on all fields of that type. AWS will validate this during deployment. Not currently a priority feature.
                // https://docs.aws.amazon.com/appsync/latest/devguide/security-authz.html#using-additional-authorization-modes

                // cache for possible future use
                _fieldTypes.Add(
                    intermediateType.Name,
                    requiredTypeInfo => intermediateType.Attribute(CreateTypeOptions(requiredTypeInfo))
                );

                return intermediateType;
            }
            finally
            {
                _typeUnderConstruction.Remove(type);
            }
        }

        private void ParseInterfaceTypeMethods(string parentName, IDictionary<string, IField> classDefinition, SystemType type)
        {
            var methods = type.GetMethodInfo();

            if (type.IsInterface)
            {
                var inheritedMethods = type.GetInterfaces().SelectMany(item => item.GetMethods());
                methods = inheritedMethods.Concat(methods);
            }

            foreach (var methodInfo in methods)
            {
                methodInfo.AssertReturnTypeIsNotNullable();
                methodInfo.AssertReturnSchemaType(type);

                var requiredTypeInfo = methodInfo.GetRequiredTypeInfo();
                var fieldMapping = methodInfo.GetFieldName(parentName);

                GraphqlType returnObjectType;

                if (IsTypeUnderConstruction(requiredTypeInfo.Type))
                {
                    // the type is already under construction - we can get away with a dummy intermediate type
                    // that has the name and no definition.
                    var typeDescriptor = requiredTypeInfo.Type.GetGraphqlTypeDescriptor();
                    var intermediateType = CreateIntermediateType(typeDescriptor);

                    returnObjectType = intermediateType.Attribute(CreateTypeOptions(requiredTypeInfo));
                }
                else
                {
                    returnObjectType =
                        GetGraphqlType(
                            fieldMapping,
                            requiredTypeInfo,
                            objectType => _graphqlApi.AddType(objectType));
                }

                // optionally specified via a custom attribute
                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);

                // Note: Directives work at the field level so you need to give the same access to the declaring type too.
                var authDirectives = methodInfo.GetAuthDirectivesOrDefault();

                if (dataSource == null)
                {
                    classDefinition.Add(
                        methodInfo.Name.GetGraphqlName(),
                        new Field(
                            new FieldOptions
                            {
                                Args = methodInfo.GetMethodArgs(_graphqlApi, this),
                                ReturnType = returnObjectType,
                                Directives = authDirectives
                            })
                    );
                }
                else
                {
                    methodInfo.RegisterRequestResponseMappings(fieldMapping, _mappingTemplates, _mappingTypeFactory);

                    classDefinition.Add(
                        methodInfo.Name.GetGraphqlName(),
                        new ResolvableField(
                            new ResolvableFieldOptions
                            {
                                DataSource = dataSource,
                                RequestMappingTemplate = _mappingTemplates.GetRequestMapping(fieldMapping),
                                ResponseMappingTemplate = _mappingTemplates.GetResponseMapping(fieldMapping),
                                Args = methodInfo.GetMethodArgs(_graphqlApi, this),
                                ReturnType = returnObjectType,
                                Directives = authDirectives
                            })
                    );
                }
            }
        }

        private bool IsTypeUnderConstruction(SystemType type)
        {
            var elementType = type.GetElementTypeIfArray();
            return _typeUnderConstruction.Contains(elementType);
        }

        private static GraphqlTypeOptions CreateTypeOptions(RequiredTypeInfo requiredTypeInfo)
        {
            return new GraphqlTypeOptions
            {
                IsRequired = requiredTypeInfo.IsRequired,
                IsList = requiredTypeInfo.IsList,
                IsRequiredList = requiredTypeInfo.IsRequiredList
            };
        }

        private static IIntermediateType CreateIntermediateType(GraphqlSchemaTypeDescriptor typeDescriptor, IDictionary<string, IField> classDefinition = null)
        {
            // todo: currently handles Input and Type - haven't yet looked at these below

            // new InterfaceType()
            // https://docs.aws.amazon.com/cdk/api/latest/dotnet/api/Amazon.CDK.AWS.AppSync.InterfaceType.html
            //
            // new UnionType()
            // https://docs.aws.amazon.com/cdk/api/latest/dotnet/api/Amazon.CDK.AWS.AppSync.UnionType.html

            classDefinition ??= new Dictionary<string, IField>();

            IIntermediateType intermediateType = typeDescriptor.SchemaType switch
            {
                GraphqlSchemaType.Input => new InputType(
                    typeDescriptor.Name,
                    new IntermediateTypeOptions
                    {
                        Definition = classDefinition
                    }),

                GraphqlSchemaType.Type => new ObjectType(
                    typeDescriptor.Name,
                    new ObjectTypeOptions
                    {
                        Definition = classDefinition,
                        Directives = typeDescriptor.Type.GetAuthDirectivesOrDefault()
                    }),

                _ => throw new InvalidOperationException($"Unexpected schema type '{typeDescriptor.SchemaType}'")
            };

            return intermediateType;
        }
    }
}