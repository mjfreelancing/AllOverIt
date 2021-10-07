using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Helpers
{
    /// <summary>Converts an object to an IDictionary{string, string} using a dot notation for nested members.</summary>
    public sealed class ObjectPropertySerializer
    {
        /// <summary>Provides options that determine how serialization of properties and their values are handled.</summary>
        public ObjectPropertySerializerOptions Options { get; }

        /// <summary>Constructor.</summary>
        /// <param name="options">Specifies options that determine how serialization of properties and their values are handled.
        /// If null, a default set of options will be used.</param>
        public ObjectPropertySerializer(ObjectPropertySerializerOptions options = default)
        {
            Options = options ?? new ObjectPropertySerializerOptions();
        }

        /// <summary>Serializes an object to an IDictionary{string, string}.</summary>
        /// <param name="instance">The object to be serialized.</param>
        /// <returns>A flat IDictionary{string, string} of all properties using a dot notation for nested members.</returns>
        public IDictionary<string, string> SerializeToDictionary(object instance)
        {
            _ = instance.WhenNotNull(nameof(instance));

            var dictionary = new Dictionary<string, string>();

            if (instance != null)
            {
                Populate(null, instance, dictionary, new List<object>());
            }

            return dictionary;
        }

        private void Populate(string prefix, object instance, IDictionary<string, string> values, IList<object> references)
        {
            switch (instance)
            {
                case IDictionary dictionary:
                    AppendDictionaryAsPropertyValues(prefix, dictionary, values, references);
                    break;

                case IEnumerable enumerable:
                    AppendEnumerableAsPropertyValues(prefix, enumerable, values, references);
                    break;

                default:
                    AppendObjectAsPropertyValues(prefix, instance, values, references);
                    break;
            }
        }

        private void AppendDictionaryAsPropertyValues(string prefix, IDictionary dictionary, IDictionary<string, string> values, IList<object> references)
        {
            var args = dictionary.GetType().GetGenericArguments();

            if (!args.Any())
            {
                // Assume IDictionary, such as from Environment.GetEnvironmentVariables(), contains values that can be converted to strings
                dictionary = dictionary.Cast<DictionaryEntry>().ToDictionary(entry => $"{entry.Key}", entry => $"{entry.Value}");
                args = dictionary.GetType().GetGenericArguments();
            }

            var keyType = args[0];
            var valueType = args[1];

            if (IgnoreType(keyType) || IgnoreType(valueType))
            {
                return;
            }

            var isClassType = keyType.IsClass && keyType != typeof(string);
            var idx = 0;

            var keyEnumerator = dictionary.Keys.GetEnumerator();
            var valueEnumerator = dictionary.Values.GetEnumerator();

            while (keyEnumerator.MoveNext())
            {
                valueEnumerator.MoveNext();

                var namePrefix = prefix.IsNullOrEmpty()
                    ? string.Empty
                    : $"{prefix}.";

                var parentReferences = new List<object>(references);

                AppendNameValue(
                    isClassType
                        ? $"{namePrefix}{keyType.GetFriendlyName()}`{idx}"
                        : $"{namePrefix}{keyEnumerator.Current}",
                    valueEnumerator.Current,
                    values,
                    parentReferences
                );

                ++idx;
            }

            if (!Options.IncludeEmptyCollections || idx != 0)
            {
                return;
            }

            AppendNameValue(prefix, Options.EmptyValueOutput, values, references);
        }

        private void AppendEnumerableAsPropertyValues(string prefix, IEnumerable enumerable, IDictionary<string, string> values, IList<object> references)
        {
            var args = enumerable.GetType().GetGenericArguments();

            if (args.Any() && IgnoreType(args[0]))
            {
                return;
            }

            var idx = 0;

            foreach (var value in enumerable)
            {
                var parentReferences = new List<object>(references);
                AppendNameValue($"{prefix}[{idx++}]", value, values, parentReferences);
            }

            if (!Options.IncludeEmptyCollections || idx != 0)
            {
                return;
            }

            AppendNameValue(prefix, Options.EmptyValueOutput, values, references);
        }

        private void AppendObjectAsPropertyValues(string prefix, object instance, IDictionary<string, string> values, IList<object> references)
        {
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
                    var name = propertyInfo.Name;

                    if (!prefix.IsNullOrEmpty())
                    {
                        name = prefix + "." + name;
                    }

                    var parentReferences = new List<object>(references);

                    AppendNameValue(name, value, values, parentReferences);
                }
            }
        }

        private void AppendNameValue(string name, object value, IDictionary<string, string> values, IList<object> references)
        {
            if (value == null)
            {
                values.Add(name, Options.NullValueOutput);
            }
            else
            {
                var type = value.GetType();

                var isString = type == typeof(string);

                if (isString && ((string)value).IsNullOrEmpty())        // null was already checked, so this only applies to empty values
                {
                    values.Add(name, Options.EmptyValueOutput);
                }
                else if (isString || type.IsValueType)
                {
                    var valueStr = $"{value}";

                    if (Options.Filter != null)
                    {
                        if (!IncludePropertyValue(Options.Filter, type, name, references, ref valueStr))
                        {
                            return;
                        }
                    }

                    values.Add(name, valueStr);
                }
                else
                {
                    if (IgnoreType(type))
                    {
                        return;
                    }

                    if (references.Contains(value))
                    {
                        throw new SelfReferenceException($"Self referencing detected at '{name}' of type '{type.GetFriendlyName()}'");
                    }

                    if (Options.Filter != null)
                    {
                        if (!IncludeProperty(Options.Filter, type, name, references))
                        {
                            return;
                        }
                    }

                    references.Add(value);
                    Populate(name, value, values, references);
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

        private static bool IncludeProperty(ObjectPropertyFilter filter, Type type, string name, IEnumerable<object> references)
        {
            filter.Type = type;
            filter.Name = name;
            filter.Chain = references.AsReadOnlyCollection();

            return filter.OnIncludeProperty();
        }

        private static bool IncludePropertyValue(ObjectPropertyFilter filter, Type type, string name, IEnumerable<object> references, ref string value)
        {
            filter.Type = type;
            filter.Name = name;
            filter.Chain = references.AsReadOnlyCollection();

            return filter.OnIncludeValue(ref value);
        }
    }
}
