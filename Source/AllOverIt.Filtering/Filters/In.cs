using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    public sealed class In<TType> : IIn<TType>
    {
        public IList<TType> Values { get; set; }

        public In()
        {
        }

        public In(IEnumerable<TType> values)
        {
            Values = values.AsList();
        }

        public static explicit operator List<TType>(In<TType> value)
        {
            return (List<TType>) value.Values;
        }

        public static implicit operator In<TType>(List<TType> values)
        {
            return new In<TType>(values);
        }
    }
}