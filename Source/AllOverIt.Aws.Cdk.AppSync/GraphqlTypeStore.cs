using AllOverIt.Aws.Cdk.AppSync.Attributes;
using AllOverIt.Aws.Cdk.AppSync.Exceptions;
using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.MappingTemplates;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using AllOverIt.Extensions;
using AllOverIt.Helpers;
using Amazon.CDK.AWS.AppSync;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EnumType = Amazon.CDK.AWS.AppSync.EnumType;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    public sealed class GraphqlTypeStore : IGraphqlTypeStore
    {
        private readonly IList<SystemType> _circularReferences = new List<SystemType>();
        private readonly GraphqlApi _graphqlApi;
        private readonly IMappingTemplates _mappingTemplates;
        private readonly IDataSourceFactory _dataSourceFactory;
        private readonly IResolverFactory _resolverFactory;

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

        public GraphqlTypeStore(GraphqlApi graphqlApi, IMappingTemplates mappingTemplates, IDataSourceFactory dataSourceFactory,
            IResolverFactory resolverFactory)
        {
            _graphqlApi = graphqlApi.WhenNotNull(nameof(graphqlApi));
            _mappingTemplates = mappingTemplates.WhenNotNull(nameof(mappingTemplates));
            _dataSourceFactory = dataSourceFactory.WhenNotNull(nameof(dataSourceFactory));
            _resolverFactory = resolverFactory.WhenNotNull(nameof(resolverFactory));
        }

        public GraphqlType GetGraphqlType(SystemType type, bool isRequired, bool isList, bool isRequiredList, 
            Action<IIntermediateType> typeCreated)
        {
            var fieldTypeCreator = GetTypeCreator(type, typeCreated);

            return fieldTypeCreator.Invoke(isRequired, isList, isRequiredList);
        }

        public GraphqlType GetGraphqlType(SystemType type, MemberInfo memberInfo, bool isRequired, bool isList, bool isRequiredList,
            Action<IIntermediateType> typeCreated)
        {
            var fieldTypeCreator = GetTypeCreator(type, memberInfo, typeCreated);

            return fieldTypeCreator.Invoke(isRequired, isList, isRequiredList);
        }

        public GraphqlType GetGraphqlType(SystemType type, ParameterInfo parameterInfo, bool isRequired, bool isList, bool isRequiredList,
            Action<IIntermediateType> typeCreated)
        {
            var fieldTypeCreator = GetTypeCreator(type, parameterInfo, typeCreated);

            return fieldTypeCreator.Invoke(isRequired, isList, isRequiredList);
        }

        // get the type creator based on a type and any class / interface level attributes
        private Func<bool, bool, bool, GraphqlType> GetTypeCreator(SystemType type, Action<IIntermediateType> typeCreated)
        {
            var isList = type.IsArray;

            var elementType = isList
                ? type.GetElementType()
                : type;

            var typeDescriptor = elementType!.GetGraphqlTypeDescriptor(type.GetTypeInfo());
            var typeName = typeDescriptor.Name;

            return GetTypeCreator(type, typeName, typeDescriptor, typeCreated);
        }

        // get the type creator based on method/property info (that may contain custom attributes) and, ultimately, the property type
        private Func<bool, bool, bool, GraphqlType> GetTypeCreator(SystemType type, MemberInfo memberInfo, Action<IIntermediateType> typeCreated)
        {
            var typeDescriptor = type.GetGraphqlTypeDescriptor(memberInfo);
            var typeName = typeDescriptor.Name;

            return GetTypeCreator(type, typeName, typeDescriptor, typeCreated);
        }

        // get the type creator based on parameter info (that may contain custom attributes) and, ultimately, the property type
        private Func<bool, bool, bool, GraphqlType> GetTypeCreator(SystemType type, ParameterInfo parameterInfo, Action<IIntermediateType> typeCreated)
        {
            var typeDescriptor = type.GetGraphqlTypeDescriptor(parameterInfo);
            var typeName = typeDescriptor.Name;

            return GetTypeCreator(type, typeName, typeDescriptor, typeCreated);
        }

        private Func<bool, bool, bool, GraphqlType> GetTypeCreator(SystemType type, string lookupTypeName, GraphqlSchemaTypeDescriptor typeDescriptor,
            Action<IIntermediateType> typeCreated)
        {
            if (!_fieldTypes.TryGetValue(lookupTypeName, out var fieldTypeCreator))
            {
                var isList = type.IsArray;

                var elementType = isList
                    ? type.GetElementType()
                    : type;

                var objectType = elementType!.IsEnum
                    ? CreateEnumType(elementType)
                    : CreateInterfaceType(elementType, typeDescriptor, typeCreated);            // parse methods and properties

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

        private IIntermediateType CreateInterfaceType(SystemType type, GraphqlSchemaTypeDescriptor typeDescriptor, Action<IIntermediateType> typeCreated)
        {
            try
            {
                if (_circularReferences.Contains(type))
                {
                    var typeNames = string.Join(" -> ", _circularReferences.Select(item => item.Name).Concat(new[] { type.Name }));
                    throw new NotSupportedException($"Not currently supporting type circular references (type '{typeNames}')");
                }

                _circularReferences.Add(type);

                var classDefinition = new Dictionary<string, IField>();

                ParseInterfaceTypeProperties(classDefinition, typeDescriptor, typeCreated);
                ParseInterfaceTypeMethods(classDefinition, type);

                // todo: currently handles Input and Type - haven't yet looked at 'interface'
                //new InterfaceType()

                var intermediateType = typeDescriptor.SchemaType == GraphqlSchemaType.Input
                    ? (IIntermediateType) new InputType(typeDescriptor.Name,
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

        private void ParseInterfaceTypeProperties(IDictionary<string, IField> classDefinition, GraphqlSchemaTypeDescriptor schemaTypeDescriptor,
            Action<IIntermediateType> typeCreated)
        {
            var type = schemaTypeDescriptor.Type;
            var properties = type.GetProperties();

            if (type.IsInterface)
            {
                var inheritedProperties = type.GetInterfaces().SelectMany(item => item.GetProperties());
                properties = properties.Concat(inheritedProperties).ToArray();
            }

            foreach (var propertyInfo in properties)
            {
                var propertyType = propertyInfo.PropertyType;

                if (propertyType.IsGenericNullableType())
                {
                    throw new SchemaException($"Unexpected nullable property '{propertyInfo.Name}' on type '{type.FullName}'. The presence of " +
                                              $"{nameof(SchemaTypeRequiredAttribute)} is used to declare a property required and its absence makes it optional.");
                }

                if (schemaTypeDescriptor.SchemaType == GraphqlSchemaType.Input && propertyType != typeof(string) && (propertyType.IsInterface || propertyType.IsClass))
                {
                    var propertyTypeDescriptor = propertyInfo.GetGraphqlPropertyDescriptor();

                    // make sure that property INPUT types are associated with a parent INPUT type (or an Aws scalar)
                    if (propertyTypeDescriptor.SchemaType != GraphqlSchemaType.Input && propertyTypeDescriptor.SchemaType != GraphqlSchemaType.Scalar)
                    {
                        throw new InvalidOperationException($"The property '{propertyInfo.Name}' is not an INPUT type ({propertyType.Name})");
                    }
                }

                var isRequired = propertyInfo.IsGqlTypeRequired();
                var isList = propertyType.IsArray;
                var isRequiredList = isList && propertyInfo.IsGqlArrayRequired();

                // create the field definition based on a property (the propertyInfo may contain custom properties)
                var fieldTypeCreator = GetTypeCreator(propertyType, propertyInfo, typeCreated);

                classDefinition.Add(propertyInfo.Name.GetGraphqlName(), fieldTypeCreator.Invoke(isRequired, isList, isRequiredList));

                // check if the field requires a resolver
                _resolverFactory.ConstructResolverIfRequired(type, propertyInfo);
            }
        }

        private void ParseInterfaceTypeMethods(IDictionary<string, IField> classDefinition, SystemType type)
        {
            var methods = type.GetMethodInfo();

            if (type.IsInterface)
            {
                var inheritedMethods = type.GetInterfaces().SelectMany(item => item.GetMethods());
                methods = methods.Concat(inheritedMethods);
            }

            foreach (var methodInfo in methods.Where(item => !item.IsSpecialName))
            {
                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);           // optionally specified via a custom attribute

                var isRequired = methodInfo.IsGqlTypeRequired();
                var isList = methodInfo.ReturnType.IsArray;
                var isRequiredList = isList && methodInfo.IsGqlArrayRequired();

                var returnObjectType =
                    GetGraphqlType(
                        methodInfo.ReturnType,
                        methodInfo,
                        isRequired,
                        isList,
                        isRequiredList,
                        objectType => _graphqlApi.AddType(objectType));

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
                    var mappingTemplateKey = methodInfo.GetFunctionName();

                    classDefinition.Add(
                        methodInfo.Name.GetGraphqlName(),
                        new ResolvableField(
                            new ResolvableFieldOptions
                            {
                                DataSource = dataSource,
                                RequestMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetRequestMapping(mappingTemplateKey)),
                                ResponseMappingTemplate = MappingTemplate.FromString(_mappingTemplates.GetResponseMapping(mappingTemplateKey)),
                                Args = methodInfo.GetMethodArgs(_graphqlApi, this),
                                ReturnType = returnObjectType
                            })
                    );
                }
            }
        }

        private static GraphqlTypeOptions CreateTypeOptions(bool isRequired, bool isList, bool isRequiredList)
        {
            return new()
            {
                IsRequired = isRequired,
                IsList = isList,
                IsRequiredList = isRequiredList
            };
        }
    }
}