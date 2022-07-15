namespace AllOverIt.Filtering.Filters
{
    public sealed class GreaterThan<TType> : IGreaterThan<TType>
    {
        public TType Value { get; set; }

        public GreaterThan()
        {
        }

        public GreaterThan(TType value)
        {
            Value = value;
        }

        public static explicit operator TType(GreaterThan<TType> value)
        {
            return value.Value;
        }

        public static implicit operator GreaterThan<TType>(TType value)
        {
            return new GreaterThan<TType>(value);
        }
    }
}