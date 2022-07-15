namespace AllOverIt.Filtering.Filters
{
    public sealed class GreaterThanOrEqual<TType> : IGreaterThanOrEqual<TType>
    {
        public TType Value { get; set; }

        public GreaterThanOrEqual()
        {
        }

        public GreaterThanOrEqual(TType value)
        {
            Value = value;
        }

        public static explicit operator TType(GreaterThanOrEqual<TType> value)
        {
            return value.Value;
        }

        public static implicit operator GreaterThanOrEqual<TType>(TType value)
        {
            return new GreaterThanOrEqual<TType>(value);
        }
    }
}