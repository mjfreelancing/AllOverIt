using System;
using System.Collections;
using AllOverIt.Extensions;
using AllOverIt.Serialization.JsonHelper.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Serialization.JsonHelper.Extensions
{
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
            if (element.TryGetValue<TValue>(propertyName, out var value))
            {
                return value;
            }

            throw CreateJsonHelperException(propertyName);
        }

        /// <summary>Tries to get an array of elements for a specified property.</summary>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="array">The array of elements for the specified property.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the property is present but it is not a list of JSON objects.</remarks>
        public static bool TryGetArray(this IElementDictionary element, string arrayPropertyName, out IEnumerable<IElementDictionary> array)
        {
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
                        throw new JsonHelperException($"The property {arrayPropertyName} is not an array of JSON objects.", exception);
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
        public static IEnumerable<IElementDictionary> GetArray(this IElementDictionary element, string arrayPropertyName)
        {
            if (element.TryGetArray(arrayPropertyName, out var array))
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
        public static bool TryGetArrayValues<TValue>(this IElementDictionary element, string arrayPropertyName, string propertyName, out IEnumerable<TValue> arrayValues)
        {
            if (element.TryGetArray(arrayPropertyName, out var array))
            {
                return TryGetManyArrayValues<TValue>(array, propertyName, out arrayValues);
            }

            arrayValues = Enumerable.Empty<TValue>();
            return false;
        }

        /// <summary>Tries to get the value of a property from each element of a specified array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="arrayValues">The value of each element in the specified array property.</param>
        /// <returns>True if the property exists, otherwise false.</returns>
        public static bool TryGetManyArrayValues<TValue>(IEnumerable<IElementDictionary> elements, string arrayPropertyName, out IEnumerable<TValue> arrayValues)
        {
            var values = new List<TValue>();

            foreach (var element in elements)
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
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified array property.</returns>
        public static IEnumerable<TValue> GetArrayValues<TValue>(this IElementDictionary element, string arrayPropertyName, string propertyName)
        {
            if (element.TryGetArrayValues<TValue>(arrayPropertyName, propertyName, out var arrayValues))
            {
                return arrayValues;
            }

            throw CreateJsonHelperException(new[] { arrayPropertyName, propertyName });
        }

        /// <summary>Try to get a child array of elements for a specified property.</summary>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childArray">The deepest child array of elements.</param>
        /// <returns>A child array of elements for a specified property.</returns>
        public static bool TryGetChildArray(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames, out IEnumerable<IElementDictionary> childArray)
        {
            var childElements = elements;

            foreach (var propertyName in arrayPropertyNames)
            {
                var elementsArray = new List<IElementDictionary>();

                foreach (var element in childElements)
                {
                    if (!element.TryGetArray(propertyName, out var array))
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
        public static IEnumerable<IElementDictionary> GetChildArray(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames)
        {
            if (elements.TryGetChildArray(arrayPropertyNames, out var childArray))
            {
                return childArray;
            }

            throw CreateJsonHelperException(arrayPropertyNames);
        }

        /// <summary>Try to get a child array of elements for a specified property.</summary>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childArray">The deepest child array of elements.</param>
        /// <returns>A child array of elements for a specified property.</returns>
        public static bool TryGetChildArray(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, out IEnumerable<IElementDictionary> childArray)
        {
            return new[] { element }.TryGetChildArray(arrayPropertyNames, out childArray);
        }

        /// <summary>Get a child array of elements for a specified property.</summary>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <returns>The deepest child array of elements.</returns>
        public static IEnumerable<IElementDictionary> GetChildArray(this IElementDictionary element, IEnumerable<string> arrayPropertyNames)
        {
            return (new[] { element }).GetChildArray(arrayPropertyNames);
        }

        /// <summary>Tries to get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="elements">The elements to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <param name="childArrayValues">The value of each element in the specified child array property.</param>
        /// <returns>True if the arrays and property exists, otherwise false.</returns>
        public static bool TryGetChildArrayValues<TValue>(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames, string childPropertyName,
            out IEnumerable<TValue> childArrayValues)
        {
            if (elements.TryGetChildArray(arrayPropertyNames, out var childArray))
            {
                return TryGetManyArrayValues<TValue>(childArray, childPropertyName, out childArrayValues);
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
        public static IEnumerable<TValue> GetChildArrayValues<TValue>(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames,
            string childPropertyName)
        {
            if (elements.TryGetChildArrayValues<TValue>(arrayPropertyNames, childPropertyName, out var childArrayValues))
            {
                return childArrayValues;
            }

            throw CreateJsonHelperException(arrayPropertyNames, childPropertyName);
        }

        /// <summary>Tries to get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <param name="childArrayValues">The value of each element in the specified child array property.</param>
        /// <returns>True if the arrays and property exists, otherwise false.</returns>
        public static bool TryGetChildArrayValues<TValue>(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, string childPropertyName,
            out IEnumerable<TValue> childArrayValues)
        {
            return new[] { element }.TryGetChildArrayValues<TValue>(arrayPropertyNames, childPropertyName, out childArrayValues);
        }

        /// <summary>Get the value of a property from each element of a specified child array property.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The elements to get the value from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value of.</param>
        /// <returns>The value of each element in the specified child array property.</returns>
        public static IEnumerable<TValue> GetChildArrayValues<TValue>(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, string childPropertyName)
        {
            return (new[] { element }).GetChildArrayValues<TValue>(arrayPropertyNames, childPropertyName);
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
