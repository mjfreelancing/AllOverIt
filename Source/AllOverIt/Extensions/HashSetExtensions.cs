using System.Collections.Generic;

namespace AllOverIt.Extensions
{
    /// <summary>Provides a variety of extension methods for <see cref="HashSet{T}"/>.</summary>
    public static class HashSetExtensions
    {
        public static void AddRange<TType>(this HashSet<TType> hashSet, IEnumerable<TType> source)
        {
            foreach (var item in source)
            {
                hashSet.Add(item);
            }
        }
    }
}