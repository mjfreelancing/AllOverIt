using AllOverIt.Exceptions;
using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AllOverIt.Helpers
{
    /// <summary>Converts an object to an IDictionary{string, string} using a dot notation for nested members.</summary>
    public sealed class ObjectPropertySerializationHelper
    {
        internal readonly List<Type> IgnoredTypes = new()
        {
            typeof(Task),
            typeof(Task<>)
        };

        public bool IncludeNulls { get; set; }

        public bool IncludeEmptyCollections { get; set; }

        public BindingOptions BindingOptions { get; set; }

        public string NullValueOutput { get; set; } = "<null>";

        public string EmptyValueOutput { get; set; } = "<empty>";

        public ObjectPropertySerializationHelper(BindingOptions bindingOptions = BindingOptions.Default)
        {
            BindingOptions = bindingOptions;
        }

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

        public void ClearIgnoredTypes()
        {
            IgnoredTypes.Clear();
        }

        public void AddIgnoredTypes(params Type[] types)
        {
            IgnoredTypes.AddRange(types);
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

                AppendNameValue(
                    isClassType
                        ? $"{namePrefix}{keyType.GetFriendlyName()}`{idx}"
                        : $"{namePrefix}{keyEnumerator.Current}",
                    valueEnumerator.Current,
                    values,
                    references
                );

                ++idx;
            }

            if (!IncludeEmptyCollections || idx != 0)
            {
                return;
            }

            AppendNameValue(prefix, EmptyValueOutput, values, references);
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
                AppendNameValue($"{prefix}[{idx++}]", value, values, references);
            }

            if (!IncludeEmptyCollections || idx != 0)
            {
                return;
            }

            AppendNameValue(prefix, EmptyValueOutput, values, references);
        }

        private void AppendObjectAsPropertyValues(string prefix, object instance, IDictionary<string, string> values, IList<object> references)
        {
            var properties = instance
                .GetType()
                .GetPropertyInfo(BindingOptions)
                .Where(propInfo => propInfo.CanRead &&
                                   !propInfo.IsIndexer() &&
                                   !IgnoreType(propInfo.PropertyType)
                );

            foreach (var propertyInfo in properties)
            {
                var value = propertyInfo.GetValue(instance);

                if (IncludeNulls || value != null)
                {
                    var name = propertyInfo.Name;

                    if (!prefix.IsNullOrEmpty())
                    {
                        name = prefix + "." + name;
                    }

                    AppendNameValue(name, value, values, references);
                }
            }
        }

        private void AppendNameValue(string name, object value, IDictionary<string, string> values, IList<object> references)
        {
            if (value == null)
            {
                values.Add(name, NullValueOutput);
            }
            else
            {
                var type = value.GetType();

                if (type.IsValueType || type == typeof(string))
                {
                    values.Add(name, $"{value}");
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

            if (IgnoredTypes.Contains(type))
            {
                return true;
            }

            return type.IsGenericType && IgnoredTypes.Contains(type.GetGenericTypeDefinition());
        }
    }
}
