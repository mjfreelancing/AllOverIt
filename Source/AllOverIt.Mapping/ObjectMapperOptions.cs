using AllOverIt.Assertion;
using AllOverIt.Extensions;

namespace AllOverIt.Mapping
{
    /// <summary>Provides global operations for all object mapping.</summary>
    internal sealed class ObjectMapperOptions : IObjectMapperOptions
    {
        private readonly ObjectMapperTypeFactory _typeFactory;

        /// <inheritdoc />
        public bool AllowNullCollections { get; set; }

        internal ObjectMapperOptions(ObjectMapperTypeFactory typeFactory)
        {
            _typeFactory = typeFactory;
        }

        /// <inheritdoc />
        public void Register<TType>()
        {
            Register<TType>(typeof(TType).GetFactory());
        }

        /// <inheritdoc />
        public void Register<TType>(Func<object> factory)
        {
            _ = factory.WhenNotNull(nameof(factory));

            _typeFactory.GetOrAdd(typeof(TType), factory);
        }
    }
}