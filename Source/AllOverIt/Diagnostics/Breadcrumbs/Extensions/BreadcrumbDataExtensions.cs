using AllOverIt.Formatters.Objects;
using System.Collections.Generic;
using System.Linq;

namespace AllOverIt.Diagnostics.Breadcrumbs.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="IEnumerable{T}"/>.</summary>
    public static class BreadcrumbDataExtensions
    {
        /// <summary>Gets all breadcrumbs messages and with the metadata serialized to a property name/value
        /// dictionary using an <see cref="ObjectPropertySerializer"/>.</summary>
        /// <param name="breadcrumbs">The breadcrumbs to be converted.</param>
        /// <param name="options">Options that control how the property serialization is handled. If not provided, a default set
        /// of options is applied that includes null values, empty collections, and collates enumerable values.</param>
        /// <returns>All breadcrumbs messages and associated metadata (as a Dictionary&lt;string, string>).</returns>
        /// <remarks>If a non-class type metadata item is added then the serialized key name will be an underscore '_'. If a collection
        /// of values is added as a metadata item and the <paramref name="options"/> is configured to collate values (the default) then the serialized
        /// key name will be '[]'.</remarks>
        public static IEnumerable<BreadcrumbSerializedData> WithSerializatedMetadata(this IEnumerable<BreadcrumbData> breadcrumbs, ObjectPropertySerializerOptions options = default)
        {
            if (options == null)
            {
                options = new ObjectPropertySerializerOptions
                {
                    IncludeNulls = true,
                    IncludeEmptyCollections = true,
                };

                options.EnumerableOptions.CollateValues = true;
            }

            var objectSerializer = new ObjectPropertySerializer(options);

            return breadcrumbs.Select(item =>
            {
                var metadata = item.Metadata;

                if (metadata != null)
                {
                    var metadataType = metadata.GetType();

                    if (!metadataType.IsClass && metadataType != typeof(string))
                    {
                        metadata = new { _ = item.Metadata };
                    }
                }

                var serializedMetadata = metadata != null
                    ? objectSerializer.SerializeToDictionary(metadata)
                    : default;

                return new BreadcrumbSerializedData
                {
                    Timestamp = item.Timestamp,
                    Message = item.Message,
                    Metadata = serializedMetadata
                };
            });
        }
    }
}
