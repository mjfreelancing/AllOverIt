using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace AllOverIt.Serialization.Abstractions
{
    public interface IJsonSerializer
    {
        /// <summary>Serializes the provided object to a JSON string.</summary>
        /// <typeparam name="TType">The object type to serialize.</typeparam>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>A JSON string representing the provided object.</returns>
        string SerializeObject<TType>(TType value);

        /// <summary>Serializes the provided object to an array of UTF8 bytes.</summary>
        /// <typeparam name="TType">The object type to serialize.</typeparam>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>An array of UTF8 bytes representing the provided object.</returns>
        byte[] SerializeToUtf8Bytes<TType>(TType value);

        /// <summary>Deserializes the provided JSON string to a specified type.</summary>
        /// <typeparam name="TType">The type to be deserialized from the provided JSON string.</typeparam>
        /// <param name="value">The value to be deserialized.</param>
        /// <returns>An instance of the specified type.</returns>
        TType DeserializeObject<TType>(string value);

        /// <summary>Deserializes the provided stream to a specified type. The stream is assumed to contain UTF8 bytes.</summary>
        /// <typeparam name="TType">The type to be deserialized from the provided stream.</typeparam>
        /// <param name="stream">The stream to be deserialized.</param>
        /// <returns>An instance of the specified type.</returns>
        Task<TType> DeserializeObjectAsync<TType>(Stream stream, CancellationToken cancellationToken);
    }
}