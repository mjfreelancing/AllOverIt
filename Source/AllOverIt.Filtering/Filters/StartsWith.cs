namespace AllOverIt.Filtering.Filters
{
    public class StartsWith : IStartsWith
    {
        public string Value { get; set; }

        public StartsWith()
        {
        }

        public StartsWith(string value)
        {
            Value = value;
        }

        public static explicit operator string(StartsWith value)
        {
            return value.Value;
        }

        public static implicit operator StartsWith(string value)
        {
            return new StartsWith
            {
                Value = value
            };
        }
    }


}