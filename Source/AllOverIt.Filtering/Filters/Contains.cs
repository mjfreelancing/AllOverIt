namespace AllOverIt.Filtering.Filters
{
    public class Contains : IContains
    {
        public string Value { get; set; }

        public Contains()
        {
        }

        public Contains(string value)
        {
            Value = value;
        }

        public static explicit operator string(Contains value)
        {
            return value.Value;
        }

        public static implicit operator Contains(string value)
        {
            return new Contains
            {
                Value = value
            };
        }
    }


}