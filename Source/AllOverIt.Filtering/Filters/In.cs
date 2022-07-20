using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    public sealed class In<TProperty> : IIn<TProperty>
    {
        public IList<TProperty> Value { get; set; }

        public In()
        {
        }

        public In(IEnumerable<TProperty> values)
        {
            Value = values.AsList();
        }

        public static explicit operator List<TProperty>(In<TProperty> value)
        {
            return (List<TProperty>) value.Value;
        }

        public static implicit operator In<TProperty>(List<TProperty> values)
        {
            return new In<TProperty>(values);
        }
    }
}