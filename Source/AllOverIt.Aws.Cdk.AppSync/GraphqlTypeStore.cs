using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Mapping;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using EnumType = Amazon.CDK.AWS.AppSync.EnumType;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    public sealed class GraphqlTypeStore
    {
        private readonly IList<SystemType> _circularReferences = new List<SystemType>();
        private readonly GraphqlApi _graphqlApi;
        private readonly MappingTemplates _mappingTemplates;
        private readonly DataSourceFactory _dataSourceFactory;

        private readonly IDictionary<string, Func<bool, bool, bool, GraphqlType>> _fieldTypes = new Dictionary<string, Func<bool, bool, bool, GraphqlType>>
        {
            {nameof(GraphqlTypeId), (isRequired, isList, isRequiredList) => GraphqlType.Id(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypePhone), (isRequired, isList, isRequiredList) => GraphqlType.AwsPhone(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypeEmail), (isRequired, isList, isRequiredList) => GraphqlType.AwsEmail(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypeIpAddress), (isRequired, isList, isRequiredList) => GraphqlType.AwsIpAddress(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypeJson), (isRequired, isList, isRequiredList) => GraphqlType.AwsJson(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypeUrl), (isRequired, isList, isRequiredList) => GraphqlType.AwsUrl(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypeTimestamp), (isRequired, isList, isRequiredList) => GraphqlType.AwsTimestamp(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypeDate), (isRequired, isList, isRequiredList) => GraphqlType.AwsDate(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypeTime), (isRequired, isList, isRequiredList) => GraphqlType.AwsTime(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(AwsTypeDateTime), (isRequired, isList, isRequiredList) => GraphqlType.AwsDateTime(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(Int32), (isRequired, isList, isRequiredList) => GraphqlType.Int(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(Double), (isRequired, isList, isRequiredList) => GraphqlType.Float(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(Single), (isRequired, isList, isRequiredList) => GraphqlType.Float(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(Boolean), (isRequired, isList, isRequiredList) => GraphqlType.Boolean(CreateTypeOptions(isRequired, isList, isRequiredList))},
            {nameof(String), (isRequired, isList, isRequiredList) => GraphqlType.String(CreateTypeOptions(isRequired, isList, isRequiredList))}
        };

        public GraphqlTypeStore(GraphqlApi graphqlApi, MappingTemplates mappingTemplates, DataSourceFactory dataSourceFactory)
        {
            _graphqlApi = graphqlApi.WhenNotNull(nameof(graphqlApi));
            _mappingTemplates = mappingTemplates.WhenNotNull(nameof(mappingTemplates));
            _dataSourceFactory = dataSourceFactory.WhenNotNull(nameof(dataSourceFactory));
        }

        public GraphqlType GetGraphqlType(string fieldName, SystemType type, bool isRequired, bool isList, bool isRequiredList, Action<IIntermediateType> typeCreated)
        {
            SchemaUtils.AssertNoProperties(type);

            var typeDescriptor = type.GetGraphqlTypeDescriptor();
            var typeName = typeDescriptor.Name;

            var fieldTypeCreator = GetTypeCreator(fieldName, type, typeName, typeDescriptor, typeCreated);

            return fieldTypeCreator.Invoke(isRequired, isList, isRequiredList);
        }

        private Func<bool, bool, bool, GraphqlType> GetTypeCreator(string parentName, SystemType type, string lookupTypeName,
            GraphqlSchemaTypeDescriptor typeDescriptor, Action<IIntermediateType> typeCreated)
        {
            if (!_fieldTypes.TryGetValue(lookupTypeName, out var fieldTypeCreator))
            {
                var isList = type.IsArray;

                var elementType = isList
                    ? type.GetElementType()
                    : type;

                var objectType = elementType!.IsEnum
                    ? CreateEnumType(elementType)
                    : CreateInterfaceType(parentName, elementType, typeDescriptor);

                // notify of type creation so it can, for example, be added to a schema
                typeCreated.Invoke(objectType);

                fieldTypeCreator = _fieldTypes[lookupTypeName];
            }

            return fieldTypeCreator;
        }

        private IIntermediateType CreateEnumType(SystemType type)
        {
            var enumType = new EnumType(type.Name, new EnumTypeOptions
            {
                Definition = type.GetEnumNames().Select(item => item.ToUpperSnakeCase()).ToArray()
            });

            _fieldTypes.Add(
                type.Name,
                (isRequired, isList, isRequiredList) => enumType.Attribute(CreateTypeOptions(isRequired, isList, isRequiredList)));

            return enumType;
        }

        private IIntermediateType CreateInterfaceType(string parentName, SystemType type, GraphqlSchemaTypeDescriptor typeDescriptor)
        {
            SchemaUtils.AssertNoProperties(type);

            try
            {
                if (_circularReferences.Contains(type))
                {
                    var typeNames = string.Join(" -> ", _circularReferences.Select(item => item.Name).Concat(new[] { type.Name }));
                    throw new NotSupportedException($"Not currently supporting type circular references (type '{typeNames}')");
                }

                _circularReferences.Add(type);

                var classDefinition = new Dictionary<string, IField>();

                ParseInterfaceTypeMethods(parentName, classDefinition, type);

                // todo: currently handles Input and Type - haven't yet looked at 'interface'
                //new InterfaceType()

                var intermediateType = typeDescriptor.SchemaType == GraphqlSchemaType.Input
                    ? (IIntermediateType)new InputType(typeDescriptor.Name,
                        new IntermediateTypeOptions
                        {
                            Definition = classDefinition
                        })
                    : new ObjectType(typeDescriptor.Name,
                        new ObjectTypeOptions
                        {
                            Definition = classDefinition
                        });

                // cache for possible future use
                _fieldTypes.Add(
                    intermediateType.Name,
                    (isRequired, isList, isRequiredList) => intermediateType.Attribute(CreateTypeOptions(isRequired, isList, isRequiredList))
                );

                return intermediateType;
            }
            finally
            {
                _circularReferences.Remove(type);
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

                var returnType = methodInfo.ReturnType;
                var isRequired = methodInfo.IsGqlTypeRequired();
                var isList = returnType.IsArray;
                var isRequiredList = isList && methodInfo.IsGqlArrayRequired();

                var fieldMapping = methodInfo.GetFieldName(parentName);

                var returnObjectType =
                    GetGraphqlType(
                        fieldMapping,
                        returnType,
                        isRequired,
                        isList,
                        isRequiredList,
                        objectType => _graphqlApi.AddType(objectType));

                // optionally specified via a custom attribute
                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);

                if (dataSource == null)
                {
                    classDefinition.Add(
                        methodInfo.Name.GetGraphqlName(),
                        new Field(
                            new FieldOptions
                            {
                                Args = methodInfo.GetMethodArgs(_graphqlApi, this),
                                ReturnType = returnObjectType
                            })
                    );
                }
                else
                {
                    methodInfo.RegisterRequestResponseMappings(fieldMapping, _mappingTemplates);

                    classDefinition.Add(
                        methodInfo.Name.GetGraphqlName(),
                        new ResolvableField(
                            new ResolvableFieldOptions
                            {
                                DataSource = dataSource,
                                RequestMappingTemplate = _mappingTemplates.GetRequestMapping(fieldMapping),
                                ResponseMappingTemplate = _mappingTemplates.GetResponseMapping(fieldMapping),
                                Args = methodInfo.GetMethodArgs(_graphqlApi, this),
                                ReturnType = returnObjectType
                            })
                    );
                }
            }
        }

        private static GraphqlTypeOptions CreateTypeOptions(bool isRequired, bool isList, bool isRequiredList)
        {
            return new GraphqlTypeOptions()
            {
                IsRequired = isRequired,
                IsList = isList,
                IsRequiredList = isRequiredList
            };
        }
    }
}