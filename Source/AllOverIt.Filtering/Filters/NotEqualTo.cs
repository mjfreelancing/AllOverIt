namespace AllOverIt.Filtering.Filters
{
    public sealed class NotEqualTo<TType> : INotEqualTo<TType>
    {
        public TType Value { get; set; }

        public NotEqualTo()
        {
        }

        public NotEqualTo(TType value)
        {
            Value = value;
        }

        public static explicit operator TType(NotEqualTo<TType> value)
        {
            return value.Value;
        }

        public static implicit operator NotEqualTo<TType>(TType value)
        {
            return new NotEqualTo<TType>(value);
        }
    }
}