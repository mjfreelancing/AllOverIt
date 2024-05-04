using AllOverIt.Assertion;
using AllOverIt.Aws.Cdk.AppSync.Extensions;
using AllOverIt.Aws.Cdk.AppSync.Factories;
using AllOverIt.Aws.Cdk.AppSync.Schema.Types;
using AllOverIt.Extensions;
using Amazon.CDK.AWS.AppSync;
using Cdklabs.AwsCdkAppsyncUtils;
using SystemType = System.Type;

namespace AllOverIt.Aws.Cdk.AppSync
{
    internal sealed class GraphqlTypeStore
    {
        private readonly List<SystemType> _typeUnderConstruction = [];

        private readonly GraphqlApi _graphqlApi;
        private readonly GraphqlSchema _schema;
        private readonly IAppGraphqlProps _apiProps;
        private readonly DataSourceFactory _dataSourceFactory;

        private readonly Dictionary<string, Func<RequiredTypeInfo, GraphqlType>> _fieldTypes = new()
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

        public GraphqlTypeStore(GraphqlApi graphqlApi, GraphqlSchema schema, IAppGraphqlProps apiProps, DataSourceFactory dataSourceFactory)
        {
            _graphqlApi = graphqlApi.WhenNotNull();
            _schema = schema.WhenNotNull();
            _apiProps = apiProps.WhenNotNull();
            _dataSourceFactory = dataSourceFactory.WhenNotNull();
        }

        public GraphqlType GetGraphqlType(string? fieldName, RequiredTypeInfo requiredTypeInfo, Action<IIntermediateType> typeCreated)
        {
            SchemaUtils.AssertNoProperties(requiredTypeInfo.Type);

            var typeDescriptor = requiredTypeInfo.Type.GetGraphqlTypeDescriptor(_apiProps.TypeNameOverrides);
            var typeName = typeDescriptor.Name;

            var fieldTypeCreator = GetTypeCreator(fieldName, requiredTypeInfo.Type, typeName, typeDescriptor, typeCreated);

            return fieldTypeCreator.Invoke(requiredTypeInfo);
        }

        private Func<RequiredTypeInfo, GraphqlType> GetTypeCreator(string? parentName, SystemType type, string lookupTypeName,
            GraphqlSchemaTypeDescriptor typeDescriptor, Action<IIntermediateType> typeCreated)
        {
            if (!_fieldTypes.TryGetValue(lookupTypeName, out var fieldTypeCreator))
            {
                var elementType = type.GetElementTypeIfArray();

                IIntermediateType objectType;

                if (elementType!.IsEnum)
                {
                    objectType = CreateEnumType(elementType, typeDescriptor);
                }
                else if (elementType.IsEnrichedEnum())
                {
                    objectType = CreateEnumTypeFromEnrichedEnum(elementType, typeDescriptor);
                }
                else
                {
                    objectType = CreateInterfaceType(parentName, elementType, typeDescriptor);
                }

                // notify of type creation so it can, for example, be added to a schema
                typeCreated.Invoke(objectType);

                fieldTypeCreator = _fieldTypes[lookupTypeName];
            }

            return fieldTypeCreator;
        }

        private EnumType CreateEnumType(SystemType type, GraphqlSchemaTypeDescriptor typeDescriptor)
        {
            var enumType = new EnumType(typeDescriptor.Name, new EnumTypeOptions
            {
                Definition = type.GetEnumNames().Select(item => item.ToUpperSnakeCase()).ToArray()
            });

            return CreateEnumType(enumType, typeDescriptor);
        }

        private EnumType CreateEnumTypeFromEnrichedEnum(SystemType type, GraphqlSchemaTypeDescriptor typeDescriptor)
        {
            var propNames = type.GetFields()
                .Where(fieldInfo => fieldInfo.IsStatic && fieldInfo.FieldType == type)
                .Select(fieldInfo => fieldInfo.Name.ToUpperSnakeCase());

            var enumType = new EnumType(typeDescriptor.Name, new EnumTypeOptions
            {
                Definition = propNames.ToArray()
            });

            return CreateEnumType(enumType, typeDescriptor);
        }

        private EnumType CreateEnumType(EnumType enumType, GraphqlSchemaTypeDescriptor typeDescriptor)
        {
            _fieldTypes.Add(
                typeDescriptor.Name,
                requiredTypeInfo => enumType.Attribute(CreateTypeOptions(requiredTypeInfo)));

            return enumType;
        }

        private IIntermediateType CreateInterfaceType(string? parentName, SystemType type, GraphqlSchemaTypeDescriptor typeDescriptor)
        {
            SchemaUtils.AssertNoProperties(type);

            try
            {
                if (_typeUnderConstruction.Contains(type))
                {
                    var typeNames = string.Join(" -> ", _typeUnderConstruction.Select(item => item.Name).Concat([type.Name]));
                    throw new InvalidOperationException($"Unexpected re-entry while creating '{typeNames}'");
                }

                _typeUnderConstruction.Add(type);

                var classDefinition = new Dictionary<string, IField>();

                ParseInterfaceTypeMethods(parentName, classDefinition, type, typeDescriptor);

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

        private void ParseInterfaceTypeMethods(string? parentName, Dictionary<string, IField> classDefinition, SystemType type, GraphqlSchemaTypeDescriptor parentTypeDescriptor)
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
                var fieldMapping = methodInfo.GetFieldName(parentName);     // Such as Query.Parent.Child.Field

                GraphqlType returnObjectType;

                if (IsTypeUnderConstruction(requiredTypeInfo.Type))
                {
                    // the type is already under construction - we can get away with a dummy intermediate type
                    // that has the name and no definition.
                    var typeDescriptor = requiredTypeInfo.Type.GetGraphqlTypeDescriptor(_apiProps.TypeNameOverrides);
                    var intermediateType = CreateIntermediateType(typeDescriptor);

                    returnObjectType = intermediateType.Attribute(CreateTypeOptions(requiredTypeInfo));
                }
                else
                {
                    returnObjectType =
                        GetGraphqlType(
                            fieldMapping,
                            requiredTypeInfo,
                            objectType => _schema.AddType(objectType));
                }

                // Note: Directives work at the field level so you need to give the same access to the declaring type too.
                var authDirectives = methodInfo.GetAuthDirectivesOrDefault();

                // Optionally specified via a custom attribute
                var fieldName = methodInfo.Name.GetGraphqlName();
                var dataSource = methodInfo.GetDataSource(_dataSourceFactory);

                classDefinition.Add(
                    fieldName,
                    new Field(
                        new FieldOptions
                        {
                            Args = methodInfo.GetMethodArgs(_schema, this),
                            ReturnType = returnObjectType,
                            Directives = authDirectives
                        })
                );

                // Can be null for subscriptions
                if (dataSource is not null)
                {
                    methodInfo.RegisterResolver(fieldMapping, _apiProps.ResolverRegistry, _apiProps.ResolverFactory);

                    var resolverProps = new ExtendedResolverProps
                    {
                        TypeName = parentTypeDescriptor.Name,
                        FieldName = fieldName,
                        DataSource = dataSource
                    };

                    _apiProps.ResolverRegistry.SetResolverProps(fieldMapping, resolverProps);

                    _graphqlApi.CreateResolver($"{parentTypeDescriptor.Name}{fieldName}Resolver", resolverProps);
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

        private static IIntermediateType CreateIntermediateType(GraphqlSchemaTypeDescriptor typeDescriptor, IDictionary<string, IField>? classDefinition = default)
        {
            // TODO: currently handles Input and Type - haven't yet looked at these below

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

                _ => throw new InvalidOperationException($"Unexpected schema type '{typeDescriptor.SchemaType}' ({typeDescriptor.Name})")
            };

            return intermediateType;
        }
    }
}