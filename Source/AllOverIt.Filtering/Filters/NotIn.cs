using AllOverIt.Extensions;
using System.Collections.Generic;

namespace AllOverIt.Filtering.Filters
{
    public sealed class NotIn<TProperty> : INotIn<TProperty>
    {
        public IList<TProperty> Value { get; set; }

        public NotIn()
        {
        }

        public NotIn(IEnumerable<TProperty> values)
        {
            Value = values.AsList();
        }

        public static explicit operator List<TProperty>(NotIn<TProperty> value)
        {
            return (List<TProperty>) value.Value;
        }

        public static implicit operator NotIn<TProperty>(List<TProperty> values)
        {
            return new NotIn<TProperty>(values);
        }
    }
}