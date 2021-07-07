using AllOverIt.Extensions;
using AllOverIt.Reflection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Helpers
{
    public sealed class ObjectPropertySerializationHelper
    {
        internal static readonly List<Type> IgnoredTypes = new()
        {
            //typeof(Task),
            //typeof(Task<>)

            //typeof(Action),
            //typeof(Action<>),
            //typeof(Action<,>),
            //typeof(Action<,,>),
            //typeof(Action<,,,>),
            //typeof(Action<,,,,>),
            //typeof(Action<,,,,,>),
            //typeof(Action<,,,,,,>),
            //typeof(Action<,,,,,,,>),
            //typeof(Action<,,,,,,,,>),
            //typeof(Action<,,,,,,,,,>),
            //typeof(Action<,,,,,,,,,,>),
            //typeof(Action<,,,,,,,,,,,>),
            //typeof(Action<,,,,,,,,,,,,>),
            //typeof(Action<,,,,,,,,,,,,,>),
            //typeof(Action<,,,,,,,,,,,,,,>),
            //typeof(Action<,,,,,,,,,,,,,,,>),
            //typeof(Func<>),
            //typeof(Func<,>),
            //typeof(Func<,,>),
            //typeof(Func<,,,>),
            //typeof(Func<,,,,>),
            //typeof(Func<,,,,,>),
            //typeof(Func<,,,,,,>),
            //typeof(Func<,,,,,,,>),
            //typeof(Func<,,,,,,,,>),
            //typeof(Func<,,,,,,,,,>),
            //typeof(Func<,,,,,,,,,,>),
            //typeof(Func<,,,,,,,,,,,>),
            //typeof(Func<,,,,,,,,,,,,>),
            //typeof(Func<,,,,,,,,,,,,,>),
            //typeof(Func<,,,,,,,,,,,,,,>),
            //typeof(Func<,,,,,,,,,,,,,,,>)
        };

        public bool IncludeNulls { get; set; }

        public bool IncludeEmptyCollections { get; set; }

        public BindingOptions BindingOptions { get; set; }

        public string NullValueOutput { get; set; } = "<null>";

        public string EmptyValueOutput { get; set; } = "<empty>";

        public ObjectPropertySerializationHelper(bool includeNulls = false, bool includeEmptyCollections = false, BindingOptions bindingOptions = BindingOptions.Default)
        {
            IncludeNulls = includeNulls;
            IncludeEmptyCollections = includeEmptyCollections;
            BindingOptions = bindingOptions;
        }

        public IDictionary<string, string> SerializeToDictionary(object instance)
        {
            _ = instance.WhenNotNull(nameof(instance));

            var dictionary = new Dictionary<string, string>();

            if (instance != null)
            {
                Populate(null, instance, dictionary);
            }

            return dictionary;
        }

        public void IgnoreTypes(params Type[] types)
        {
            IgnoredTypes.AddRange(types);
        }

        private void Populate(string prefix, object instance, IDictionary<string, string> values)
        {
            switch (instance)
            {
                case IDictionary dictionary:
                    AppendDictionaryAsPropertyValues(prefix, dictionary, values);
                    break;

                case IEnumerable enumerable:
                    AppendEnumerableAsPropertyValues(prefix, enumerable, values);
                    break;

                default:
                    AppendObjectAsPropertyValues(prefix, instance, values);
                    break;
            }
        }

        private void AppendDictionaryAsPropertyValues(string prefix, IDictionary dictionary, IDictionary<string, string> values)
        {
            var args = dictionary.GetType().GetGenericArguments();
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

                AppendNameValue(isClassType
                        ? $"{namePrefix}{keyType.GetFriendlyName()}`{idx}"
                        : $"{namePrefix}{keyEnumerator.Current}", valueEnumerator.Current, values
                );

                ++idx;
            }

            if (!IncludeEmptyCollections || idx != 0)
            {
                return;
            }

            AppendNameValue(prefix, EmptyValueOutput, values);
        }

        private void AppendEnumerableAsPropertyValues(string prefix, IEnumerable enumerable, IDictionary<string, string> values)
        {
            var args = enumerable.GetType().GetGenericArguments();

            if (args.Any() && IgnoreType(args[0]))
            {
                return;
            }

            var idx = 0;

            foreach (var value in enumerable)
            {
                AppendNameValue($"{prefix}[{idx++}]", value, values);
            }

            if (!IncludeEmptyCollections || idx != 0)
            {
                return;
            }

            AppendNameValue(prefix, EmptyValueOutput, values);
        }

        private void AppendObjectAsPropertyValues(string prefix, object instance, IDictionary<string, string> values)
        {
            foreach (var propertyInfo in instance.GetType().GetPropertyInfo(BindingOptions).Where(prop => prop.CanRead))
            {
                var value = propertyInfo.GetValue(instance);

                if (IncludeNulls || value != null)
                {
                    var name = propertyInfo.Name;

                    if (!prefix.IsNullOrEmpty())
                    {
                        name = prefix + "." + name;
                    }

                    AppendNameValue(name, value, values);
                }
            }
        }

        private void AppendNameValue(string name, object value, IDictionary<string, string> values)
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

                    Populate(name, value, values);
                }
            }
        }

        private static bool IgnoreType(Type type)
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
