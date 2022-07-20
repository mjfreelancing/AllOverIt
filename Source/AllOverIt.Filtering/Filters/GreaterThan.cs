namespace AllOverIt.Filtering.Filters
{
    public sealed class GreaterThan<TProperty> : IGreaterThan<TProperty>
    {
        public TProperty Value { get; set; }

        public GreaterThan()
        {
        }

        public GreaterThan(TProperty value)
        {
            Value = value;
        }

        public static explicit operator TProperty(GreaterThan<TProperty> value)
        {
            return value.Value;
        }

        public static implicit operator GreaterThan<TProperty>(TProperty value)
        {
            return new GreaterThan<TProperty>(value);
        }
    }
}