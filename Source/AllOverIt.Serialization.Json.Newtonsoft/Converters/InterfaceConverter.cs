using AllOverIt.Extensions;
using Newtonsoft.Json.Converters;

namespace AllOverIt.Serialization.Json.Newtonsoft.Converters
{
    /// <summary>Deserializes an interface to a concrete type.</summary>
    /// <typeparam name="TInterface">The interface type to convert from.</typeparam>
    /// <typeparam name="TConcrete">The concrete type to convert to.</typeparam>
    public class InterfaceConverter<TInterface, TConcrete> : CustomCreationConverter<TInterface>
        where TConcrete : class, TInterface
    {
        private static readonly Func<object> _factory = typeof(TConcrete).GetFactory();

        /// <summary>Constructor.</summary>
        public InterfaceConverter()
            : base()
        {
        }

        /// <inheritdoc />
        public override TInterface Create(Type objectType)
        {
            return (TInterface) _factory.Invoke();
        }
    }
}
