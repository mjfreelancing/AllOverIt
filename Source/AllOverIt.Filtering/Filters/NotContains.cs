namespace AllOverIt.Filtering.Filters
{
    public sealed class NotContains : INotContains
    {
        public string Value { get; set; }

        public NotContains()
        {
        }

        public NotContains(string value)
        {
            Value = value;
        }

        public static explicit operator string(NotContains value)
        {
            return value.Value;
        }

        public static implicit operator NotContains(string value)
        {
            return new NotContains
            {
                Value = value
            };
        }
    }
}