using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    public sealed class NotIn<TType> : INotIn<TType>
    {
        public IList<TType> Values { get; set; }

        public NotIn()
        {
        }

        public NotIn(IEnumerable<TType> values)
        {
            Values = values.AsList();
        }

        public static explicit operator List<TType>(NotIn<TType> value)
        {
            return (List<TType>) value.Values;
        }

        public static implicit operator NotIn<TType>(List<TType> values)
        {
            return new NotIn<TType>(values);
        }
    }
}