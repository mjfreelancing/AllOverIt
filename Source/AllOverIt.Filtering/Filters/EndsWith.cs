namespace AllOverIt.Filtering.Filters
{
    public sealed class EndsWith : IEndsWith
    {
        public string Value { get; set; }

        public EndsWith()
        {
        }

        public EndsWith(string value)
        {
            Value = value;
        }

        public static explicit operator string(EndsWith value)
        {
            return value.Value;
        }

        public static implicit operator EndsWith(string value)
        {
            return new EndsWith
            {
                Value = value
            };
        }
    }
}