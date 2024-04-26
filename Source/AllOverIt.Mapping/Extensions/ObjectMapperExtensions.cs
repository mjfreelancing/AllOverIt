using AllOverIt.Assertion;
using System.Collections;

namespace AllOverIt.Mapping.Extensions
{
    /// <summary>Provides extension methods for mapping a source type to a target type.</summary>
    public static class ObjectMapperExtensions
    {
        /// <summary>Maps a collection of source objects to a collection of target objects. The source elements
        /// must all be mappable to the target type.</summary>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="mapper">The object mapper instance.</param>
        /// <param name="sources">The source elements.</param>
        /// <returns>An <see cref="IEnumerable"/> of elements mapped to the <typeparamref name="TTarget"/> type.</returns>
        public static IEnumerable<TTarget> MapMany<TTarget>(this IObjectMapper mapper, IEnumerable sources)
           where TTarget : class, new()
        {
            _ = mapper.WhenNotNull(nameof(mapper));
            _ = sources.WhenNotNull(nameof(sources));

            var enumerator = sources.GetEnumerator();

            while (enumerator.MoveNext())
            {
                yield return mapper.Map<TTarget>(enumerator.Current);
            }
        }

        /// <summary>Maps a collection of source objects to a collection of target objects.</summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="mapper">The object mapper instance.</param>
        /// <param name="sources">The source elements.</param>
        /// <returns>An <see cref="IEnumerable"/> of elements mapped to the <typeparamref name="TTarget"/> type.</returns>
        public static IEnumerable<TTarget> MapMany<TSource, TTarget>(this IObjectMapper mapper, IEnumerable<TSource> sources)
            where TSource : class
            where TTarget : class, new()
        {
            _ = mapper.WhenNotNull(nameof(mapper));
            _ = sources.WhenNotNull(nameof(sources));

            return sources.Select(source =>
            {
                var target = new TTarget();
                return mapper.Map(source, target);
            });
        }
    }
}