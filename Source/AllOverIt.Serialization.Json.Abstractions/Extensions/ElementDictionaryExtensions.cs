using AllOverIt.Assertion;
using AllOverIt.Extensions;
using AllOverIt.Serialization.Json.Abstractions.Exceptions;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

namespace AllOverIt.Serialization.Json.Abstractions.Extensions
{
    // Note: Tested indirectly via JsonHelper (SystemTextJson and NewtonsoftJson) - consider writing similar explicit tests for these extensions.

    /// <summary>Provides extension methods for <see cref="IElementDictionary"/>.</summary>
    public static class ElementDictionaryExtensions
    {
        /// <summary>Tries to get the value of a specified property on an element. If the value is not a <typeparamref name="TValue"/>
        /// then an attempt will be made to convert it using <see cref="ObjectExtensions.As{TType}(object, TType?)"/>.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="propertyName">The name of the property on the element to get the value from.</param>
        /// <param name="value">The value of the property, if the property exists.</param>
        /// <returns><see langword="True" /> if the property exists, otherwise <see langword="False" />.</returns>
        public static bool TryGetValue<TValue>(this IElementDictionary element, string propertyName, [MaybeNull] out TValue value)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = propertyName.WhenNotNull(nameof(propertyName));

            value = default;

            if (!element.TryGetValue(propertyName, out var @object))
            {
                return false;
            }

            if (@object is null)
            {
                return true;
            }

            value = @object is TValue objectValue
                ? objectValue
                : @object.As<TValue>();

            return true;
        }

        /// <summary>Gets the value of a specified property on an element. If the value is not a <typeparamref name="TValue"/>
        /// then an attempt will be made to convert it using <see cref="ObjectExtensions.As{TType}(object, TType?)"/>.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="propertyName">The name of the property on the element to get the value from.</param>
        /// <returns>The value of the specified property.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the property does not exist.</remarks>
        [return: MaybeNull]
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

        /// <summary>Tries to get an array of values for a specified property on an element. If the element's values are not a
        /// <typeparamref name="TValue"/> then an attempt will be made to convert each value using
        /// <see cref="ObjectExtensions.As{TType}(object, TType?)"/>.</summary>
        /// <typeparam name="TValue">The array's element type.</typeparam>
        /// <param name="element">The element to get the values from.</param>
        /// <param name="propertyName">The name of the array property on the element to get the values from.</param>
        /// <param name="values">The array of values for the specified property, if it exists, otherwise <see langword="null" />.
        /// This can also be <see langword="null" /> if the property exists, but is itself <see langword="null" />.</param>
        /// <returns><see langword="True" /> if the property exists and it is a collection that can have its values returned
        /// as an array, otherwise <see langword="False" />.</returns>
        public static bool TryGetValues<TValue>(this IElementDictionary element, string propertyName, out TValue?[]? values)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = propertyName.WhenNotNull(nameof(propertyName));

            values = default;

            if (element.TryGetValue(propertyName, out var array))
            {
                if (array is null)
                {
                    return true;
                }

                if (array is List<TValue> typedValues)
                {
                    values = [.. typedValues];

                    return true;
                }

                if (array is IEnumerable arrayItems)
                {
                    var castValues = new List<TValue?>();

                    foreach (var arrayItem in arrayItems)
                    {
                        var value = arrayItem.As<TValue>();
                        castValues.Add(value);
                    }

                    values = [.. castValues];

                    return true;
                }
            }

            return false;
        }

        /// <summary>Gets an array of values for a specified property on an element. If the element's values are not a <typeparamref name="TValue"/>
        /// then an attempt will be made to convert each value using <see cref="ObjectExtensions.As{TType}(object, TType?)"/>.</summary>
        /// <typeparam name="TValue">The array's element type.</typeparam>
        /// <param name="element">The element to get the values from.</param>
        /// <param name="propertyName">The name of the array property on the element to get the values from.</param>
        /// <returns>The array of values for a specified property.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the property does not exist
        /// or the property's value cannot be returned as an array of values.</remarks>
        public static TValue?[]? GetValues<TValue>(this IElementDictionary element, string propertyName)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = propertyName.WhenNotNull(nameof(propertyName));

            if (element.TryGetValues<TValue>(propertyName, out var values))
            {
                return values;
            }

            throw CreateJsonHelperException(propertyName);
        }

        /// <summary>Tries to get an array of elements for a specified property on an element.</summary>
        /// <param name="element">The element to get the array of elements from.</param>
        /// <param name="arrayPropertyName">The name of the array property on the element to get the array of elements from.</param>
        /// <param name="elements">The array of non-<see langword="null" /> elements for the specified array property. If the
        ///  <paramref name="arrayPropertyName"/> property does not exist, this will be <see langword="null"/>. This can also be
        ///  <see langword="null" /> if the property exists, but is itself <see langword="null" />..</param>
        /// <returns><see langword="True" /> if the array property exists, otherwise <see langword="False" />.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the property is present but it is not an array of objects.</remarks>
        public static bool TryGetObjectArray(this IElementDictionary element, string arrayPropertyName, out IElementDictionary[]? elements)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            elements = null;

            if (element.TryGetValue(arrayPropertyName, out var list))
            {
                if (list is null)
                {
                    return true;
                }

                if (list is IList items)
                {
                    try
                    {
                        // The ElementDictionary ctor will throw ArgumentNullException if any items are null.
                        // Not supporting this, but it could be somewhat handled by adding an optional predicate to the method
                        // so null (or other) elements could be filtered out.
                        elements = items
                            .Cast<Dictionary<string, object?>>()
                            .SelectToArray(item => new ElementDictionary(item));

                        return true;
                    }
                    catch (InvalidCastException exception)
                    {
                        throw new JsonHelperException($"The property {arrayPropertyName} is not an array of objects.", exception);
                    }
                }

                throw new JsonHelperException($"The property {arrayPropertyName} is not an array type.");
            }

            return false;
        }

        /// <summary>Gets an array of non-<see langword="null" /> elements for a specified property on an element.</summary>
        /// <param name="element">The element to get the array of elements from.</param>
        /// <param name="arrayPropertyName">The name of the array property on the element to get the array of elements from.</param>
        /// <returns>The array of non-<see langword="null" /> elements for the specified property.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the property does not exist, or
        /// if the property is present but it is not an array of objects.</remarks>
        public static IElementDictionary[]? GetObjectArray(this IElementDictionary element, string arrayPropertyName)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            if (element.TryGetObjectArray(arrayPropertyName, out var array))
            {
                return array;
            }

            throw CreateJsonHelperException(arrayPropertyName);
        }

        /// <summary>Tries to get the value of a property from each element in an element's array property.</summary>
        /// <typeparam name="TValue">The array's element type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name to get the value from each element in the array.</param>
        /// <param name="propertyValues">The value of each element in the specified array property. This will be
        /// <see langword="null"/> if the method returns <see langword="False"/> or the array property is <see langword="null" />.</param>
        /// <returns><see langword="True" /> if the array property exists and all elements have the required <paramref name="propertyName"/>,
        /// otherwise <see langword="False" />.</returns>
        public static bool TryGetObjectArrayValues<TValue>(this IElementDictionary element, string arrayPropertyName, string propertyName,
            out TValue?[]? propertyValues)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            propertyValues = null;

            if (element.TryGetObjectArray(arrayPropertyName, out var array))
            {
                return array is null || array.TryGetManyObjectValues(propertyName, out propertyValues);
            }

            return false;
        }

        /// <summary>Gets the value of a property from each element in an element's array property.</summary>
        /// <typeparam name="TValue">The array's element type.</typeparam>
        /// <param name="element">The element to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <param name="propertyName">The property name to get the value from each element in the array.</param>
        /// <returns>The value of each element in the specified array property, or <see langword="null"/> if the array property
        /// is <see langword="null" />.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the array property does not exist, or
        /// it is not an array of objects, or not all objects have the required property name.</remarks>
        public static TValue?[]? GetObjectArrayValues<TValue>(this IElementDictionary element, string arrayPropertyName, string propertyName)
        {
            _ = element.WhenNotNull(nameof(element));
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            if (element.TryGetObjectArrayValues<TValue>(arrayPropertyName, propertyName, out var arrayValues))
            {
                return arrayValues;
            }

            throw CreateJsonHelperException([arrayPropertyName, propertyName]);
        }

        /// <summary>Tries to get the value of a property from a collection of elements.</summary>
        /// <typeparam name="TValue">The property's value type.</typeparam>
        /// <param name="elements">The collection of elements to get the value from.</param>
        /// <param name="propertyName">The name of the property to get the value from.</param>
        /// <param name="propertyValues">An array containing the value of each element's property. This will be
        /// <see langword="null"/> if the method returns <see langword="False"/>.</param>
        /// <returns><see langword="True" /> if the property exists on all elements, otherwise <see langword="False" />.</returns>
        public static bool TryGetManyObjectValues<TValue>(this IEnumerable<IElementDictionary> elements, string propertyName,
            out TValue?[]? propertyValues)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            _ = propertyName.WhenNotNullOrEmpty(nameof(propertyName));

            var values = new List<TValue?>();

            foreach (var element in allElements)
            {
                if (!element.TryGetValue<TValue>(propertyName, out var value))
                {
                    propertyValues = null;

                    return false;
                }

                values.Add(value);
            }

            propertyValues = [.. values];

            return true;
        }

        /// <summary>Gets the value of a property from a collection of elements.</summary>
        /// <typeparam name="TValue">The property's value type.</typeparam>
        /// <param name="elements">The collection of elements to get the value from.</param>
        /// <param name="arrayPropertyName">The property name of the array element.</param>
        /// <returns>The value of each element in the specified array property.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the array property does not exist.</remarks>
        public static TValue?[]? GetManyObjectArrayValues<TValue>(IEnumerable<IElementDictionary> elements, string arrayPropertyName)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            _ = arrayPropertyName.WhenNotNullOrEmpty(nameof(arrayPropertyName));

            if (allElements.TryGetManyObjectValues<TValue>(arrayPropertyName, out var arrayValues))
            {
                return arrayValues;
            }

            throw CreateJsonHelperException([arrayPropertyName]);
        }

        /// <summary>Tries to get an array of non-<see langword="null" /> child elements from a nested path of array elements.</summary>
        /// <param name="elements">The collection of elements to get the nested child elements from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childArray">The deepest child array of non-<see langword="null" /> elements.</param>
        /// <returns>An array of non-<see langword="null" /> child elements from a nested path of array elements.</returns>
        public static bool TryGetDescendantObjectArray(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames,
            out IElementDictionary[]? childArray)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();

            /*
             * To get each 'node3' from 'elements' which contains the 'node1' items
             * 'arrayPropertyNames' will contain ['node2', 'node3']
             
                    node0 (object)
                     |
                     |
                     +------node1 (object)
                     |        |
                     |        +------node2 (object)
                     |        |       |
                     |        |       +------node3 (object)
                     |        |       |
                     |        |       +------node3 (object)
                     |        |
                     |        +------node2 (object)
                     |                |
                     |                +------node3 (object)
                     |                |
                     |                +------node3 (object)
                     |
                     |
                     +------node1 (object)
                              |
                              +------node2 (object)
                              |       |
                              |       +------node3 (object)
                              |       |
                              |       +------node3 (object)
                              |
                              +------node2 (object)
                                      |
                                      +------node3 (object)
                                      |
                                      +------node3 (object)
             */

            childArray = null;

            var childElements = allElements;

            // Drill down into the nested (child) property names per element
            foreach (var propertyName in allPropertyNames)
            {
                var elementsArray = new List<IElementDictionary>();

                foreach (var element in childElements)
                {
                    // Only supporting non-null elements
                    if (!element.TryGetObjectArray(propertyName, out var array) || array is null)
                    {
                        return false;
                    }

                    elementsArray.AddRange(array);
                }

                childElements = elementsArray;
            }

            childArray = [.. childElements];

            return true;
        }

        /// <summary>Get an array of non-<see langword="null" /> child elements from a nested path of array elements.</summary>
        /// <param name="elements">The collection of elements to get the nested child elements from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <returns>An array of non-<see langword="null" /> child elements from a nested path of array elements.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the nested path of array properties do not exist.</remarks>
        public static IElementDictionary[]? GetDescendantObjectArray(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();

            if (allElements.TryGetDescendantObjectArray(allPropertyNames, out var childArray))
            {
                return childArray;
            }

            throw CreateJsonHelperException(allPropertyNames);
        }

        /// <summary>Tries to get an array of non-<see langword="null" /> child elements from a nested path of array elements.</summary>
        /// <param name="element">The element to get the nested child elements from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childArray">The deepest child array of non-<see langword="null" /> elements.</param>
        /// <returns>An array of non-<see langword="null" /> child elements from a nested path of array elements.</returns>
        public static bool TryGetDescendantObjectArray(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, out IElementDictionary[]? childArray)
        {
            _ = element.WhenNotNull(nameof(element));
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();

            /*
             * To get each 'node3' from 'node0' (element)
             * 'arrayPropertyNames' will contain ['node1', 'node2', 'node3']

                    node0 (object)
                     |
                     |
                     +------node1 (object)
                     |        |
                     |        +------node2 (object)
                     |        |       |
                     |        |       +------node3 (object)
                     |        |       |
                     |        |       +------node3 (object)
                     |        |
                     |        +------node2 (object)
                     |                |
                     |                +------node3 (object)
                     |                |
                     |                +------node3 (object)
                     |
                     |
                     +------node1 (object)
                              |
                              +------node2 (object)
                              |       |
                              |       +------node3 (object)
                              |       |
                              |       +------node3 (object)
                              |
                              +------node2 (object)
                                      |
                                      +------node3 (object)
                                      |
                                      +------node3 (object)
             */

            return new[] { element }.TryGetDescendantObjectArray(allPropertyNames, out childArray);
        }

        /// <summary>Get an array of non-<see langword="null" /> child elements from a nested path of array elements.</summary>
        /// <param name="element">The element to get the nested child elements from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <returns>An array of non-<see langword="null" /> child elements from a nested path of array elements.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the nested path of array properties do not exist.</remarks>
        public static IElementDictionary[]? GetDescendantObjectArray(this IElementDictionary element, IEnumerable<string> arrayPropertyNames)
        {
            _ = element.WhenNotNull(nameof(element));
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames));

            return (new[] { element }).GetDescendantObjectArray(allPropertyNames);
        }

        /// <summary>Tries to get the value of a property from each element of an array of non-<see langword="null" /> child elements
        /// from a nested path of array elements.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="elements">The collection of elements to get the nested child elements from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value from.</param>
        /// <param name="childValues">The value of each element in the deepest child element.</param>
        /// <returns><see langword="True" /> if the arrays and property exists, otherwise <see langword="False" />.</returns>
        public static bool TryGetDescendantObjectArrayValues<TValue>(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames,
            string childPropertyName, out TValue?[]? childValues)
        {
            var allElements = elements.WhenNotNull(nameof(elements)).AsReadOnlyCollection();
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();
            _ = childPropertyName.WhenNotNullOrEmpty(nameof(childPropertyName));

            childValues = null;

            if (allElements.TryGetDescendantObjectArray(allPropertyNames, out var childArray))
            {
                return childArray is null || childArray.TryGetManyObjectValues(childPropertyName, out childValues);
            }

            return false;
        }

        /// <summary>Get the the value of a property from each element of an array of non-<see langword="null" /> child elements
        /// from a nested path of array elements.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="elements">The collection of elements to get the nested child elements from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value from.</param>
        /// <returns>The value of each element in the deepest child element.</returns>
        /// <remarks>A <seealso cref="JsonHelperException"/> exception will be thrown if the nested path of array properties do not exist.</remarks>
        public static TValue?[]? GetDescendantObjectArrayValues<TValue>(this IEnumerable<IElementDictionary> elements, IEnumerable<string> arrayPropertyNames,
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

        /// <summary>Tries to get the value of a property from a nested path of non-<see langword="null" /> child elements.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the nested child elements from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value from.</param>
        /// <param name="childArrayValues">The value of each element in the specified child array property. This will be
        /// <see langword="null"/> if the method returns <see langword="False"/>.</param>
        /// <returns><see langword="True" /> if the nested child element arrays and property exists, otherwise <see langword="False" />.</returns>
        public static bool TryGetDescendantObjectArrayValues<TValue>(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, string childPropertyName,
            out TValue?[]? childArrayValues)
        {
            _ = element.WhenNotNull(nameof(element));
            var allPropertyNames = arrayPropertyNames.WhenNotNull(nameof(arrayPropertyNames)).AsReadOnlyCollection();
            _ = childPropertyName.WhenNotNullOrEmpty(nameof(childPropertyName));

            return new[] { element }.TryGetDescendantObjectArrayValues<TValue>(allPropertyNames, childPropertyName, out childArrayValues);
        }

        /// <summary>Gets the value of a property from a nested path of non-<see langword="null" /> child elements.</summary>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="element">The element to get the nested child elements from.</param>
        /// <param name="arrayPropertyNames">One or more nested child array property names.</param>
        /// <param name="childPropertyName">The property name of the child element to get the value from.</param>
        /// <returns>The value of each element in the specified child array property. This will be
        /// <see langword="null"/> if the method returns <see langword="False"/>.</returns>
        public static TValue?[]? GetDescendantObjectArrayValues<TValue>(this IElementDictionary element, IEnumerable<string> arrayPropertyNames, string childPropertyName)
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
                : propertyNames.Concat([propertyName]);

            return new JsonHelperException($"The property {string.Join(".", allProperties)} was not found.");
        }
    }
}
