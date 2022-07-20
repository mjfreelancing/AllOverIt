namespace AllOverIt.Filtering.Filters
{
    public sealed class EqualTo<TProperty> : IEqualTo<TProperty>
    {
        public TProperty Value { get; set; }

        public EqualTo()
        {
        }

        public EqualTo(TProperty value)
        {
            Value = value;
        }

        public static explicit operator TProperty(EqualTo<TProperty> value)
        {
            return value.Value;
        }

        public static implicit operator EqualTo<TProperty>(TProperty value)
        {
            return new EqualTo<TProperty>(value);
        }
    }
}