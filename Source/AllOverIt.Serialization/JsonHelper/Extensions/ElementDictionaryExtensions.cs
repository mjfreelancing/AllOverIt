using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.JsonHelper.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Serialization.JsonHelper.Extensions
{
    // TODO: Currently tested indirectly via JsonHelper (SystemTextJson and NewtonsoftJson) - consider writing similar explicit tests for these extensions.

    /// <summary>Provides extension methods for <see cref="IElementDictionary"/>.</summary>
    public static class ElementDictionaryExtensions
    {
        /// <summary>Tries to get the value of a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="value">The value of the property, if the property exists.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        public static bool TryGetValue<TValue>(this IElementDictionary element, string propertyName, out TValue value)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = propertyName.WhenNotNull(nameof(propertyName));

            if (element.TryGetValue(propertyName, out var @object))
            {
                if (@object is TValue objectValue)
                {
                    value = objectValue;
                }
                else
                {
                    value = @object.As<TValue>();
                }

                return true;
            }

            value = default;
            return false;
        }

        /// <summary>Gets the value of a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <returns>The value of a specified property.</returns>
        public static TValue GetValue<TValue>(this IElementDictionary element, string propertyName)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = propertyName.WhenNotNull(nameof(propertyName));

            if (element.TryGetValue<TValue>(propertyName, out var value))
            {
                return value;
            }

            throw CreateJsonHelperException(propertyName);
        }

        /// <summary>Tries to get the array of values for a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="propertyName">The property name to get the values of.</param>
        /// <param name="values">The array values of the property, if the property exists.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        public static bool TryGetValues<TValue>(this IElementDictionary element, string propertyName, out IReadOnlyCollection<TValue> values)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = propertyName.WhenNotNull(nameof(propertyName));

            if (element.TryGetValue(propertyName, out var array))
            {
                if (array is List<TValue> typedValues)
                {
                    values = typedValues;
                    return true;
                }

                var castValues = new List<TValue>();

                foreach (var arrayItem in (IEnumerable) array)
                {
                    var value = arrayItem.As<TValue>();
                    castValues.Add(value);
                }

                values = castValues;
                return true;
            }

            values = default;
            return false;
        }

        /// <summary>Gets the array of values for a specified property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="propertyName">The property name to get the values of.</param>
        /// <returns>The array of values for a specified property.</returns>
        public static IReadOnlyCollection<TValue> GetValues<TValue>(this IElementDictionary element, string propertyName)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = propertyName.WhenNotNull(nameof(propertyName));

            if (element.TryGetValues<TValue>(propertyName, out var values))
            {
                return values;
            }

            throw CreateJsonHelperException(propertyName);
        }

        /// <summary>Tries to get an array of elements for a specified property.</summary>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="array">The array of elements for the specified property.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the property is present but it is not a list of JSON objects.</remarks>
        public static bool TryGetObjectArray(this IElementDictionary element, string arrayPropertyName, out IEnumerable<IElementDictionary> array)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            if (element.TryGetValue(arrayPropertyName, out var list))
            {
                if (list is IList items)
                {
                    try
                    {
                        array = items
                            .Cast<Dictionary<string, object>>()
                            .SelectAsReadOnlyCollection(item => new ElementDictionary(item));

                        return true;
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new JsonHelperException($"The property {arrayPropertyName} is not an array of objects.", exception);
                    }
                }

                throw new JsonHelperException($"The property {arrayPropertyName} is not an array type.");
            }

            array = Enumerable.Empty<IElementDictionary>();
            return false;
        }

        /// <summary>Gets an array of elements for a specified property.</summary>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <returns>The array of elements for the specified property.</returns>
        public static IEnumerable<IElementDictionary> GetObjectArray(this IElementDictionary element, string arrayPropertyName)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            if (element.TryGetObjectArray(arrayPropertyName, out var array))
            {
                return array;
            }

            throw CreateJsonHelperException(arrayPropertyName);
        }

        /// <summary>Tries to get the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name to get the value of.</param>
        /// <param name="arrayValues">The value of each element in the specified array property.</param>
        /// <returns>True if the array and property exists, otherwise false.</returns>
        public static bool TryGetObjectArrayValues<TValue>(this IElementDictionary element, string arrayPropertyName, string propertyName,
            out IEnumerable<TValue> arrayValues)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            if (element.TryGetObjectArray(arrayPropertyName, out var array))
            {
                return TryGetManyObjectArrayValues<TValue>(array, propertyName, out arrayValues);
            }

            arrayValues = Enumerable.Empty<TValue>();
            return false;
        }

        /// <summary>Gets the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified array property.</returns>
        public static IEnumerable<TValue> GetObjectArrayValues<TValue>(this IElementDictionary element, string arrayPropertyName, string propertyName)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            if (element.TryGetObjectArrayValues<TValue>(arrayPropertyName, propertyName, out var arrayValues))
            {
                return arrayValues;
            }

            throw CreateJsonHelperException(new[] { arrayPropertyName, propertyName });
        }

        /// <summary>Tries to get the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="arrayValues">The value of each element in the specified array property.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        public static bool TryGetManyObjectArrayValues<TValue>(this IEnumerable<IElementDictionary> elements, string arrayPropertyName,
            out IEnumerable<TValue> arrayValues)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            var values = new List<TValue>();

            foreach (var element in allElements)
            {
                if (!element.TryGetValue<TValue>(arrayPropertyName, out var childValue))
                {
                    arrayValues = Enumerable.Empty<TValue>();
                    return false;
                }

                values.Add(childValue);
            }

            arrayValues = values;
            return true;
        }

        /// <summary>Gets the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <returns>The value of each element in the specified array property.</returns>
        public static IEnumerable<TValue> GetManyObjectArrayValues<TValue>(IEnumerable<IElementDictionary> elements, string arrayPropertyName)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            if (allElements.TryGetManyObjectArrayValues<TValue>(arrayPropertyName, out var arrayValues))
            {
                return arrayValues;
            }

            throw CreateJsonHelperException(new[] { arrayPropertyName });
        }

        /// <summary>Try to get a child array of elements for a specified property.</summary>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childArray">The deepest child array of elements.</param>
        /// <returns>A child array of elements for a specified property.</returns>
        public static bool TryGetDescendantObjectArray(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames,
            out IEnumerable<IElementDictionary> childArray)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();

            var childElements = allElements;

            foreach (var propertyName in allPropertyNames)
            {
                var elementsArray = new List<IElementDictionary>();

                foreach (var element in childElements)
                {
                    if (!element.TryGetObjectArray(propertyName, out var array))
                    {
                        childArray = Enumerable.Empty<IElementDictionary>();
                        return false;
                    }

                    elementsArray.AddRange(array);
                }

                childElements = elementsArray;
            }

            childArray = childElements;
            return true;
        }

        /// <summary>Get a child array of elements for a specified property.</summary>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <returns>The deepest child array of elements.</returns>
        public static IEnumerable<IElementDictionary> GetDescendantObjectArray(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();

            if (allElements.TryGetDescendantObjectArray(allPropertyNames, out var childArray))
            {
                return childArray;
            }

            throw CreateJsonHelperException(allPropertyNames);
        }

        /// <summary>Try to get a child array of elements for a specified property.</summary>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childArray">The deepest child array of elements.</param>
        /// <returns>A child array of elements for a specified property.</returns>
        public static bool TryGetDescendantObjectArray(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, out IEnumerable<IElementDictionary> childArray)
        {
            _ = element.WhenNotNull(nameof(element));
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();

            return new[] { element }.TryGetDescendantObjectArray(allPropertyNames, out childArray);
        }

        /// <summary>Get a child array of elements for a specified property.</summary>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <returns>The deepest child array of elements.</returns>
        public static IEnumerable<IElementDictionary> GetDescendantObjectArray(this IElementDictionary element, IEnumerable<string> arrayPropertyNames)
        {
            _ = element.WhenNotNull(nameof(element));
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames));

            return (new[] { element }).GetDescendantObjectArray(allPropertyNames);
        }

        /// <summary>Tries to get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <param name="childArrayValues">The value of each element in the specified child array property.</param>
        /// <returns>True if the arrays and property exists, otherwise false.</returns>
        public static bool TryGetDescendantObjectArrayValues<TValue>(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames,
            string childPropertyName, out IEnumerable<TValue> childArrayValues)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();
            _ = childPropertyName.WhenNotNullOrEmpty(nameof(childPropertyName));

            if (allElements.TryGetDescendantObjectArray(allPropertyNames, out var childArray))
            {
                return TryGetManyObjectArrayValues<TValue>(childArray, childPropertyName, out childArrayValues);
            }

            childArrayValues = Enumerable.Empty<TValue>();
            return false;
        }

        /// <summary>Get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified child array property.</returns>
        public static IEnumerable<TValue> GetDescendantObjectArrayValues<TValue>(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames,
            string childPropertyName)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();
            _ = childPropertyName.WhenNotNullOrEmpty(nameof(childPropertyName));

            if (allElements.TryGetDescendantObjectArrayValues<TValue>(allPropertyNames, childPropertyName, out var childArrayValues))
            {
                return childArrayValues;
            }

            throw CreateJsonHelperException(allPropertyNames, childPropertyName);
        }

        /// <summary>Tries to get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <param name="childArrayValues">The value of each element in the specified child array property.</param>
        /// <returns>True if the arrays and property exists, otherwise false.</returns>
        public static bool TryGetDescendantObjectArrayValues<TValue>(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, string childPropertyName,
            out IEnumerable<TValue> childArrayValues)
        {
            _ = element.WhenNotNull(nameof(element));
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();
            _ = childPropertyName.WhenNotNullOrEmpty(nameof(childPropertyName));

            return new[] { element }.TryGetDescendantObjectArrayValues<TValue>(allPropertyNames, childPropertyName, out childArrayValues);
        }

        /// <summary>Get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The elements to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified child array property.</returns>
        public static IEnumerable<TValue> GetDescendantObjectArrayValues<TValue>(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, string childPropertyName)
        {
            _ = element.WhenNotNull(nameof(element));
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();
            _ = childPropertyName.WhenNotNullOrEmpty(nameof(childPropertyName));

            return (new[] { element }).GetDescendantObjectArrayValues<TValue>(allPropertyNames, childPropertyName);
        }

        private static JsonHelperException CreateJsonHelperException(string propertyName)
        {
            return new JsonHelperException($"The property {propertyName} was not found.");
        }

        private static JsonHelperException CreateJsonHelperException(IEnumerable<string> propertyNames)
        {
            return CreateJsonHelperException(propertyNames, string.Empty);
        }

        private static JsonHelperException CreateJsonHelperException(IEnumerable<string> propertyNames, string propertyName)
        {
            var allProperties = propertyName.IsNullOrEmpty()
                ? propertyNames
                : propertyNames.Concat(new[] { propertyName });

            return new JsonHelperException($"The property {string.Join(".", allProperties)} was not found.");
        }
    }
}
