namespace AllOverIt.Filtering.Filters
{
    public sealed class LessThan<TProperty> : ILessThan<TProperty>
    {
        public TProperty Value { get; set; }

        public LessThan()
        {
        }

        public LessThan(TProperty value)
        {
            Value = value;
        }

        public static explicit operator TProperty(LessThan<TProperty> value)
        {
            return value.Value;
        }

        public static implicit operator LessThan<TProperty>(TProperty value)
        {
            return new LessThan<TProperty>
            {
                Value = value
            };
        }
    }


}