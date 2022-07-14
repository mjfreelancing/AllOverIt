namespace EFEnumerationDemo.Filtering
{

    public class GreaterThan<TType> : IGreaterThan<TType>
    {
        public TType Value { get; set; }

        public GreaterThan()
        {
        }

        public GreaterThan(TType value)
        {
            Value = value;
        }

        public static implicit operator TType(GreaterThan<TType> value)
        {
            return value.Value;
        }

        public static explicit operator GreaterThan<TType>(TType value)
        {
            return new GreaterThan<TType>(value);
        }
    }

    public class LessThan<TType> : ILessThan<TType>
    {
        public TType Value { get; set; }

        public LessThan()
        {
        }

        public LessThan(TType value)
        {
            Value = value;
        }

        public static implicit operator TType(LessThan<TType> value)
        {
            return value.Value;
        }

        public static explicit operator LessThan<TType>(TType value)
        {
            return new LessThan<TType>
            {
                Value = value
            };
        }
    }

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

        public static implicit operator string(Contains value)
        {
            return value.Value;
        }

        public static explicit operator Contains(string value)
        {
            return new Contains
            {
                Value = value
            };
        }
    }

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

        public static implicit operator string(StartsWith value)
        {
            return value.Value;
        }

        public static explicit operator StartsWith(string value)
        {
            return new StartsWith
            {
                Value = value
            };
        }
    }


    public interface IFilter
    {
    }

    public class Filter : IFilter
    {
        public GreaterThan<int> GreaterThan { get; init; } = new();
        public LessThan<int> LessThan { get; init; } = new();
        public Contains Contains { get; init; } = new();
        public StartsWith StartsWith { get; init; } = new();
    }


}