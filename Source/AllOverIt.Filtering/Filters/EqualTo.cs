namespace AllOverIt.Filtering.Filters
{
    public sealed class EqualTo<TType> : IEqualTo<TType>
    {
        public TType Value { get; set; }

        public EqualTo()
        {
        }

        public EqualTo(TType value)
        {
            Value = value;
        }

        public static explicit operator TType(EqualTo<TType> value)
        {
            return value.Value;
        }

        public static implicit operator EqualTo<TType>(TType value)
        {
            return new EqualTo<TType>(value);
        }
    }
}