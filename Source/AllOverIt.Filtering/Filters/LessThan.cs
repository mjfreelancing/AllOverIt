namespace AllOverIt.Filtering.Filters
{
    public sealed class LessThan<TType> : ILessThan<TType>
    {
        public TType Value { get; set; }

        public LessThan()
        {
        }

        public LessThan(TType value)
        {
            Value = value;
        }

        public static explicit operator TType(LessThan<TType> value)
        {
            return value.Value;
        }

        public static implicit operator LessThan<TType>(TType value)
        {
            return new LessThan<TType>
            {
                Value = value
            };
        }
    }


}