namespace AllOverIt.Filtering.Filters
{
    public sealed class NotEqualTo<TProperty> : INotEqualTo<TProperty>
    {
        public TProperty Value { get; set; }

        public NotEqualTo()
        {
        }

        public NotEqualTo(TProperty value)
        {
            Value = value;
        }

        public static explicit operator TProperty(NotEqualTo<TProperty> value)
        {
            return value.Value;
        }

        public static implicit operator NotEqualTo<TProperty>(TProperty value)
        {
            return new NotEqualTo<TProperty>(value);
        }
    }
}