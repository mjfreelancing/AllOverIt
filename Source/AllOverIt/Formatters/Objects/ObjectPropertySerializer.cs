using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Formatters.Objects.Exceptions;
using AllOverIt.Reflection;
using System.Collections;

namespace AllOverIt.Formatters.Objects
{
    /// <inheritdoc cref="IObjectPropertySerializer" />
    public sealed class ObjectPropertySerializer : IObjectPropertySerializer
    {
        /// <inheritdoc />
        public ObjectPropertySerializerOptions Options { get; }

        /// <inheritdoc />
        public ObjectPropertyFilter? Filter { get; } = default;

        /// <summary>Constructor. Applies a default constructed <see cref="ObjectPropertySerializerOptions"/>.</summary>
        /// <param name="filter">Provides support for filtering properties and values when serializing. Optional.</param>
        public ObjectPropertySerializer()
            : this(new ObjectPropertySerializerOptions(), null)
        {
        }

        /// <summary>Constructor. Applies a default constructed <see cref="ObjectPropertySerializerOptions"/>.</summary>
        /// <param name="filter">Provides support for filtering properties and values when serializing. Optional.</param>
        public ObjectPropertySerializer(ObjectPropertyFilter filter)
            : this(new ObjectPropertySerializerOptions(), filter)
        {
        }

        /// <summary>Constructor.</summary>
        /// <param name="options">Specifies options that determine how serialization of properties and their values are handled.
        /// If null, a default set of options will be used.</param>
        /// <param name="filter">Provides support for filtering properties and values when serializing. Optional.</param>
        public ObjectPropertySerializer(ObjectPropertySerializerOptions options, ObjectPropertyFilter? filter = default)
        {
            Options = options.WhenNotNull(nameof(options));
            Filter = filter;
        }

        /// <inheritdoc />
        public IDictionary<string, string> SerializeToDictionary(object instance)
        {
            _ = instance.WhenNotNull(nameof(instance));

            if (instance is IDictionary<string, string> dictionary)
            {
                return dictionary;
            }

            dictionary = new Dictionary<string, string>();

            if (instance != null)
            {
                Populate(string.Empty, instance, dictionary, new Dictionary<object, ObjectPropertyParent>());
            }

            return dictionary;
        }

        private void Populate(string prefix, object instance, IDictionary<string, string> values, IDictionary<object, ObjectPropertyParent> references)
        {
            switch (instance)
            {
                case IDictionary dictionary:
                    AppendDictionaryAsPropertyValues(prefix, dictionary, values, references);
                    break;

                case IEnumerable enumerable when instance.GetType() != CommonTypes.StringType:
                    var collateValues = CanCollateEnumerableValues(enumerable, references);

                    var arrayValues = collateValues
                        ? new Dictionary<string, string>()
                        : values;

                    AppendEnumerableAsPropertyValues(prefix, enumerable, arrayValues, references);

                    if (collateValues)
                    {
                        if (prefix.IsEmpty())
                        {
                            // The array must have been a root object (not a property value) so use "[]" as the prefix
                            prefix = Options.RootValueOptions.ArrayKeyName;
                        }

                        values.Add(prefix, string.Join(GetEnumerableOptions().Separator, arrayValues.Values));
                    }
                    break;

                default:
                    AppendObjectAsPropertyValues(prefix, instance, values, references);
                    break;
            }
        }

        private void AppendDictionaryAsPropertyValues(string prefix, IDictionary dictionary, IDictionary<string, string> values,
            IDictionary<object, ObjectPropertyParent> references)
        {
            if (ExcludeDictionary(dictionary))
            {
                return;
            }

            var args = GetDictionaryGenericArguments(dictionary);
            var keyType = args[0];

            var isClassType = keyType.IsClass && keyType != CommonTypes.StringType;
            var idx = 0;

            var keyEnumerator = dictionary.Keys.GetEnumerator();
            var valueEnumerator = dictionary.Values.GetEnumerator();

            while (keyEnumerator.MoveNext())
            {
                valueEnumerator.MoveNext();

                var namePrefix = prefix.IsNullOrEmpty()
                    ? string.Empty
                    : $"{prefix}.";

                var parentReferences = new Dictionary<object, ObjectPropertyParent>(references);

                AppendNameValue(
                    isClassType
                        ? $"{namePrefix}{keyType.GetFriendlyName()}`{idx}"
                        : $"{namePrefix}{keyEnumerator.Current}",
                    null,
                    valueEnumerator.Current,
                    idx,
                    values,
                    parentReferences
                );

                ++idx;
            }

            if (!Options.IncludeEmptyCollections || idx != 0)
            {
                return;
            }

            // output an empty value
            AppendNameValue(prefix, null, Options.EmptyValueOutput, null, values, references);
        }

        private void AppendEnumerableAsPropertyValues(string prefix, IEnumerable enumerable, IDictionary<string, string> values,
            IDictionary<object, ObjectPropertyParent> references)
        {
            if (ExcludeEnumerable(enumerable))
            {
                return;
            }

            var idx = 0;

            foreach (var value in enumerable)
            {
                var parentReferences = new Dictionary<object, ObjectPropertyParent>(references);
                AppendNameValue($"{prefix}[{idx}]", null, value, idx, values, parentReferences);
                idx++;
            }

            if (!Options.IncludeEmptyCollections || idx != 0)
            {
                return;
            }

            // output an empty value
            AppendNameValue(prefix, null, Options.EmptyValueOutput, null, values, references);
        }

        private void AppendObjectAsPropertyValues(string? prefix, object instance, IDictionary<string, string> values,
            IDictionary<object, ObjectPropertyParent> references)
        {
            var instanceType = instance.GetType();

            if (!instanceType.IsClass || instanceType == CommonTypes.StringType)
            {
                // The value doesn't have properties....only option is to ToString() it
                values.Add(Options.RootValueOptions.ScalarKeyName, instance.ToString()!);
                return;
            }

            var properties = instance
                .GetType()
                .GetPropertyInfo(Options.BindingOptions)
                .Where(propInfo => propInfo.CanRead &&
                                   !propInfo.IsIndexer() &&
                                   !IgnoreType(propInfo.PropertyType));

            foreach (var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(instance);

                if (Options.IncludeNulls || value != null)
                {
                    var fullPath = propertyInfo.Name;

                    if (!prefix.IsNullOrEmpty())
                    {
                        fullPath = prefix + "." + fullPath;
                    }

                    var parentReferences = new Dictionary<object, ObjectPropertyParent>(references);

                    AppendNameValue(fullPath, propertyInfo.Name, value, null, values, parentReferences);
                }
            }
        }

        private void AppendNameValue(string path, string? name, object? value, int? index, IDictionary<string, string> values,
            IDictionary<object, ObjectPropertyParent> references)
        {
            if (value is null)
            {
                values.Add(path, Options.NullValueOutput);
            }
            else
            {
                var type = value.GetType();

                var isString = type == CommonTypes.StringType;

                if (isString && ((string) value).IsEmpty())        // null was already checked, so this only applies to empty values
                {
                    values.Add(path, Options.EmptyValueOutput);
                }
                else if (isString || type.IsValueType)
                {
                    var valueStr = value.ToString()!;

                    if (Filter is not null)
                    {
                        if (!IncludePropertyValue(type, value, path, name, index, references))
                        {
                            return;
                        }

                        if (Filter is IFormattableObjectPropertyFilter formattable)
                        {
                            valueStr = formattable.OnFormatValue(valueStr);
                        }
                    }

                    values.Add(path, valueStr);
                }
                else
                {
                    if (IgnoreType(type))
                    {
                        return;
                    }

                    if (references.ContainsKey(value))
                    {
                        throw new SelfReferenceException($"Self referencing detected at '{path}' of type '{type.GetFriendlyName()}'.");
                    }

                    if (Filter is not null)
                    {
                        if (ExcludeValueType(value) || !IncludeProperty(type, value, path, name, index, references))
                        {
                            return;
                        }
                    }

                    var parent = new ObjectPropertyParent(name, value, index);
                    references.Add(value, parent);

                    Populate(path, value, values, references);
                }
            }
        }

        private bool IgnoreType(Type type)
        {
            if (typeof(Delegate).IsAssignableFrom(type.BaseType))
            {
                return true;
            }

            if (Options.IgnoredTypes.Contains(type))
            {
                return true;
            }

            return type.IsGenericType && Options.IgnoredTypes.Contains(type.GetGenericTypeDefinition());
        }

        private bool ExcludeValueType(object instance)
        {
            return instance switch
            {
                IDictionary dictionary => ExcludeDictionary(dictionary),
                IEnumerable enumerable => ExcludeEnumerable(enumerable),
                _ => false,
            };
        }

        private static Type[] GetDictionaryGenericArguments(IDictionary dictionary)
        {
            var args = dictionary.GetType().GetGenericArguments();

            if (args.NotAny())
            {
                // Assume IDictionary, such as from Environment.GetEnvironmentVariables(), contains values that can be converted to strings
                dictionary = dictionary.Cast<DictionaryEntry>().ToDictionary(
                    entry => entry.Key.ToString()!,
                    entry => entry.Value!.ToString()!);

                args = dictionary.GetType().GetGenericArguments();
            }

            return args;
        }

        private bool ExcludeDictionary(IDictionary dictionary)
        {
            var args = GetDictionaryGenericArguments(dictionary);

            var keyType = args[0];
            var valueType = args[1];

            return IgnoreType(keyType) || IgnoreType(valueType);
        }

        private bool ExcludeEnumerable(IEnumerable enumerable)
        {
            var args = enumerable.GetType().GetGenericArguments();

            return args.Length != 0 && IgnoreType(args[0]);
        }

        private void SetFilterAttributes(Type type, object value, string path, string? name, int? index, IDictionary<object, ObjectPropertyParent> references)
        {
            var propertyPath = GetPropertyPath(references);

            if (!name.IsNullOrEmpty())
            {
                propertyPath = propertyPath.IsNullOrEmpty()
                    ? name
                    : $"{propertyPath}.{name}";
            }

            Filter!.Type = type;
            Filter.Value = value;
            Filter.Path = path;
            Filter.PropertyPath = propertyPath;
            Filter.Name = name;     // Is null when iterating over a collection (Index will be non-null)
            Filter.Index = index;
            Filter.Parents = references.Values.AsReadOnlyCollection();
        }

        private bool IncludeProperty(Type type, object value, string path, string? name, int? index, IDictionary<object, ObjectPropertyParent> references)
        {
            SetFilterAttributes(type, value, path, name, index, references);

            return Filter!.OnIncludeProperty();
        }

        private bool IncludePropertyValue(Type type, object value, string path, string? name, int? index, IDictionary<object, ObjectPropertyParent> references)
        {
            return IncludeProperty(type, value, path, name, index, references) && Filter!.OnIncludeValue();
        }

        private static string GetPropertyPath(IDictionary<object, ObjectPropertyParent> references)
        {
            return string.Join(".", references.Values.Where(item => item.Name != null).Select(item => item.Name));
        }

        private static Type GetEnumerableElementType(IEnumerable enumerable)
        {
            var enumerableType = enumerable.GetType();

            // See if it's an array (will return null if not an array)
            var elementType = enumerableType.GetElementType();

            // Otherwise a generic collection
            if (elementType is null && enumerableType.IsGenericType)
            {
                elementType = enumerableType.GetGenericArguments()[0];
            }

            return elementType ?? CommonTypes.ObjectType;
        }

        private bool CanCollateEnumerableValues(IEnumerable enumerable, IDictionary<object, ObjectPropertyParent> references)
        {
            // Only allowing the collation of primitive types - collating complex types is not very helpful / readable
            var elementType = GetEnumerableElementType(enumerable);

            if (elementType.IsClassType() && elementType != CommonTypes.StringType)
            {
                return false;
            }

            // Check if the filter indicates collation is required
            var collateValues = GetEnumerableOptions().CollateValues;

            if (!collateValues)
            {
                var globalCollatePaths = Options.EnumerableOptions.AutoCollatedPaths ?? Enumerable.Empty<string>();
                var filterCollatePaths = Filter?.EnumerableOptions.AutoCollatedPaths ?? Enumerable.Empty<string>();
                var collationPaths = globalCollatePaths.Concat(filterCollatePaths).AsReadOnlyCollection();

                if (collationPaths.Count != 0)
                {
                    // Check if the current path is registered for auto-collation
                    var flatPath = GetPropertyPath(references);
                    collateValues = collationPaths.Contains(flatPath);
                }
            }

            return collateValues;
        }

        private ObjectPropertyEnumerableOptions GetEnumerableOptions()
        {
            // If there's a filter, its options override the serializer's global array handling options
            return Filter?.EnumerableOptions ?? Options.EnumerableOptions;
        }
    }
}
