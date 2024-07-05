using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Serialization.Binary.Readers
{
    /// <inheritdoc cref="IEnrichedBinaryValueReader"/>
    public abstract class EnrichedBinaryValueReader : IEnrichedBinaryValueReader
    {
        private readonly Func<object> _typeFactory;

        /// <inheritdoc />
        public Type Type { get; }

        /// <summary>Constructor.</summary>
        /// <param name="type">The value type read by this value reader.</param>
        public EnrichedBinaryValueReader(Type type)
        {
            Type = type.WhenNotNull(nameof(type));

            _typeFactory = type.GetFactory();
        }

        /// <inheritdoc />
        public abstract object ReadValue(IEnrichedBinaryReader reader);

        protected object CreateType()
        {
            return _typeFactory.Invoke();
        }
    }
}
