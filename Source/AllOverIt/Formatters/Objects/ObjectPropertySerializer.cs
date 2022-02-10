﻿using AllOverIt.Assertion;
using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AllOverIt.Patterns.ResourceInitialization;

namespace AllOverIt.Formatters.Objects
{
    /// <summary>Converts an object to an IDictionary&lt;string, string&gt; using a dot notation for nested members.</summary>
    public sealed class ObjectPropertySerializer : IObjectPropertySerializer
    {
        /// <inheritdoc />
        public ObjectPropertySerializerOptions Options { get; }

        /// <summary>Constructor.</summary>
        /// <param name="options">Specifies options that determine how serialization of properties and their values are handled.
        /// If null, a default set of options will be used.</param>
        public ObjectPropertySerializer(ObjectPropertySerializerOptions options = default)
        {
            Options = options ?? new ObjectPropertySerializerOptions();
        }

        /// <inheritdoc />
        public IDictionary<string, string> SerializeToDictionary(object instance)
        {
            _ = instance.WhenNotNull(nameof(instance));

            var dictionary = new Dictionary<string, string>();

            if (instance != null)
            {
                Populate(null, instance, dictionary, new Dictionary<object, ObjectPropertyParent>());
            }

            return dictionary;
        }

        private void Populate(string prefix, object instance, IDictionary<string, string> values, IDictionary<object, ObjectPropertyParent> references)
        {
            var collateValues = (Options.Filter?.EnumerableOptions ?? Options.EnumerableOptions).CollateValues;

            switch (instance)
            {
                case IDictionary dictionary:
                    AppendDictionaryAsPropertyValues(prefix, dictionary, values, references);
                    break;

                case IEnumerable enumerable:
                    var arrayValues = collateValues
                        ? new Dictionary<string, string>()
                        : values;

                    AppendEnumerableAsPropertyValues(prefix, enumerable, arrayValues, references);

                    if (collateValues)
                    {
                        values.Add(prefix, string.Join(Options.EnumerableOptions.Separator, arrayValues.Values));
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
            // ReSharper disable once PossibleMultipleEnumeration
            if (ExcludeEnumerable(enumerable))
            {
                return;
            }

            var idx = 0;

            // ReSharper disable once PossibleMultipleEnumeration
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

        private void AppendObjectAsPropertyValues(string prefix, object instance, IDictionary<string, string> values,
            IDictionary<object, ObjectPropertyParent> references)
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

        private void AppendNameValue(string path, string name, object value, int? index, IDictionary<string, string> values,
            IDictionary<object, ObjectPropertyParent> references)
        {
            if (value == null)
            {
                values.Add(path, Options.NullValueOutput);
            }
            else
            {
                var type = value.GetType();

                var isString = type == typeof(string);

                if (isString && ((string)value).IsNullOrEmpty())        // null was already checked, so this only applies to empty values
                {
                    values.Add(path, Options.EmptyValueOutput);
                }
                else if (isString || type.IsValueType)
                {
                    var valueStr = $"{value}";

                    if (Options.Filter != null)
                    {
                        if (!IncludePropertyValue(type, value, path, name, index, references))
                        {
                            return;
                        }

                        if (Options.Filter is IFormattableObjectPropertyFilter formattable)
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
                        throw new SelfReferenceException($"Self referencing detected at '{path}' of type '{type.GetFriendlyName()}'");
                    }

                    if (Options.Filter != null)
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

            if (!args.Any())
            {
                // Assume IDictionary, such as from Environment.GetEnvironmentVariables(), contains values that can be converted to strings
                dictionary = dictionary.Cast<DictionaryEntry>().ToDictionary(entry => $"{entry.Key}", entry => $"{entry.Value}");
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
            return args.Any() && IgnoreType(args[0]);
        }

        private void SetFilterAttributes(Type type, object value, string path, string name, int? index, IDictionary<object, ObjectPropertyParent> references)
        {
            Options.Filter.Type = type;
            Options.Filter.Value = value;
            Options.Filter.Path = path;
            Options.Filter.Name = name;
            Options.Filter.Index = index;
            Options.Filter.Parents = references.Values.AsReadOnlyCollection();
        }

        private bool IncludeProperty(Type type, object value, string path, string name, int? index, IDictionary<object, ObjectPropertyParent> references)
        {
            SetFilterAttributes(type, value, path, name, index, references);

            return Options.Filter.OnIncludeProperty();
        }

        private bool IncludePropertyValue(Type type, object value, string path, string name, int? index, IDictionary<object, ObjectPropertyParent> references)
        {
            return IncludeProperty(type, value, path, name, index, references) && Options.Filter.OnIncludeValue();
        }
    }
}
