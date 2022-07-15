namespace AllOverIt.Filtering.Filters
{
    public sealed class LessThanOrEqual<TType> : ILessThanOrEqual<TType>
    {
        public TType Value { get; set; }

        public LessThanOrEqual()
        {
        }

        public LessThanOrEqual(TType value)
        {
            Value = value;
        }

        public static explicit operator TType(LessThanOrEqual<TType> value)
        {
            return value.Value;
        }

        public static implicit operator LessThanOrEqual<TType>(TType value)
        {
            return new LessThanOrEqual<TType>(value);
        }
    }
}