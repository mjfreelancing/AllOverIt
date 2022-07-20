namespace AllOverIt.Filtering.Filters
{
    public sealed class GreaterThanOrEqual<TProperty> : IGreaterThanOrEqual<TProperty>
    {
        public TProperty Value { get; set; }

        public GreaterThanOrEqual()
        {
        }

        public GreaterThanOrEqual(TProperty value)
        {
            Value = value;
        }

        public static explicit operator TProperty(GreaterThanOrEqual<TProperty> value)
        {
            return value.Value;
        }

        public static implicit operator GreaterThanOrEqual<TProperty>(TProperty value)
        {
            return new GreaterThanOrEqual<TProperty>(value);
        }
    }
}